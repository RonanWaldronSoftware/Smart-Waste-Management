using SmartWaste.Model.Constants;
using SmartWaste.Model.Core;
using SmartWaste.Model.DataTransferObjects.Dashboard;
using SmartWaste.Model.Enums;
using SmartWaste.Model.Extensions;
using SmartWaste.Model.Helpers;
using SmartWaste.Model.JsonModels.Unit;
using SmartWaste.Repository.Base;
using SmartWaste.Repository.Contracts;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.Repository.Implementation
{
    public class UnitRepository : RepositoryBase<Unit>, IUnitRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public UnitRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public void CreateUnit(Unit unit) => Create(unit);


        public async Task CreateUnitAsync(Unit unit)
        {
            Create(unit);
            await _repositoryContext.SaveChangesAsync();
        }

        public async Task<List<Unit>> CreateAUnitList(List<Unit> newUnits)
        {
            CreateList(newUnits);

            await _repositoryContext.SaveChangesAsync();

            return newUnits;
        }

        public async Task<int> GetUnitNavigationCount(List<int> possibleParentsIds, bool trackChanges = true)
        {
            return await FindAll(trackChanges)
                .Where(u => possibleParentsIds.Contains(u.UserId.Value)
                && !u.Deleted
                && u.StatusId != DatabaseConstants.UnitStatus.Inactive.Id
                && u.LatestUnitReadings
                    .Any(ur => ur.CreateDate < DateTime.UtcNow.AddDays(-1)
                            || ur.BatteryUvlo           
                            || ur.BinEmptiedLastPeriod
                            || ur.BatteryOverTempLo
                            || ur.BinLocked
                            || ur.BinTilted
                            || ur.ServiceDoorOpen
                            || ur.FlapStuckOpen
                            || ur.BinFull
                            || ur.RegularPeriodicReporting == false)).CountAsync();
        }

        public async Task<Unit> GetUnit(string serialNumber, bool trackChanges = true, bool isSigfox = false)
        => await FindByCondition(u => (isSigfox ? u.SigFoxAltId.Equals(serialNumber) : u.SerialNumber.Equals(serialNumber)) && !u.Deleted, trackChanges)
            .Include(u => u.UnitModelType)
            .FirstOrDefaultAsync();

        public async Task<Unit?> GetUnit(int unitId, bool trackChanges = true, bool includeRelated = true)
        {
            var query = FindByCondition(u => u.Id == unitId && !u.Deleted, trackChanges);

            if (includeRelated)
            {
                query = query.Include(u => u.ContentType)
                             .Include(u => u.User)
                             .Include(u => u.UnitType)
                             .Include(u => u.RouteUnits.Where(ru=> !ru.Route.Deleted))
                             .Include(u => u.UnitImages)
                             .Include(u => u.UnitModelType)
                             .Include(u => u.LatestUnitReadings);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<Unit>> GetNotReportingUnits() => await _repositoryContext.Units
                .Where(u => (u.LatestUnitReadings != null && u.LatestUnitReadings.All(r => r.CreateDate.AddHours(24) >= DateTime.UtcNow)) && u.Deleted == false
                && u.StatusId == DatabaseConstants.UnitStatus.Active.Id).ToListAsync();

        public async Task<TableResponse<Unit>> GetUnits(GetUnitsInput input, List<int> possibleParentsIds, bool trackChanges = true)
        {
            var options = input.DataTableOptions;

            var query = FindAll(trackChanges)
                .Include(u => u.User)
                .Include(u => u.ContentType)
                .Include(u => u.LatestUnitReadings)
                .Where(u => (input.IncludeInactive || u.StatusId != DatabaseConstants.UnitStatus.Inactive.Id)
                            && possibleParentsIds.Contains(u.UserId.Value) && !u.Deleted &&
                            (EF.Functions.Like(u.SerialNumber, $"%{options.Search}%")
                            || EF.Functions.Like(u.User.Name, $"%{options.Search}%")
                            || EF.Functions.Like(u.Location, $"%{options.Search}%")));

            // Apply filters
            if (input.Filter is not null)
            {
                var filter = input.Filter;

                query = query.Where(u =>
                    filter.ContentTypeIds.Contains(u.ContentTypeId.Value) &&
                    (!u.LatestUnitReadings.Any() || filter.FullnessStatusIds.Contains(u.LatestUnitReadings.First().FullnessStatusId)) &&
                    filter.UnitTypesIds.Contains(u.UnitTypeId.Value) &&
                    filter.UnitModelTypeIds.Contains(u.UnitModelTypeId.Value) &&
                    filter.UserIds.Contains(u.UserId.Value));
            }

            // Check if sorting is by bin level
            if (options.SortBy.ToLower().Equals("binlevel"))
            {
                query = options.SortByDirection.ToLower().Equals("asc") 
                    ? query.OrderBy(u => u.LatestUnitReadings.Any() 
                        ? u.LatestUnitReadings.First().BinLevelPercentage 
                        : null)
                    : query.OrderByDescending(u => u.LatestUnitReadings.Any() 
                        ? u.LatestUnitReadings.First().BinLevelPercentage 
                        : null);
                options.SortBy = string.Empty;
                options.SortByDirection = string.Empty;
            }

            options.TotalRecords = await query.CountAsync();

            return new TableResponse<Unit>
            {
                Responses = await query.OrderByPage(options.SortBy, options.SortByDirection, options.Page, options.PerPage).ToListAsync(),
                Options = options
            };
        }

        public async Task<List<Unit>> GetUnitsForCustomer(int userid, bool trackChanges = true)
        => await FindAll(trackChanges).Where(u => !u.Deleted && u.StatusId == DatabaseConstants.UnitStatus.Active.Id && u.UserId == userid).ToListAsync();

        public async Task<bool> IsSerialNumberExist(string serialNumber) => await Any(u => u.SerialNumber.Equals(serialNumber) && !u.Deleted);

        public async Task<DashboardDto> GetDashboardStatistic(List<int> possibleParentsIds, bool trackChanges = true)
        {
            var dashboardDto = new DashboardDto();

            // Define content types to process
            var contentTypes = new List<ContentTypeEnum>
                {
                    ContentTypeEnum.MixWaste,
                    ContentTypeEnum.Recycling,
                };

            // Process each content type
            foreach (var contentType in contentTypes)
            {
                dashboardDto.ContentTypesCount.Add(new ContentTypeCountDto
                {
                    ContentTypeId = (int)contentType,
                    Count = await _repositoryContext.Units.AsNoTracking().CountAsync(u => possibleParentsIds.Any(id => id == u.UserId) && !u.Deleted && u.ContentTypeId == (int)contentType)
                });

                // Counts for graph
                var smallFullnessCount = await GetFullnessCountAsync(contentType, possibleParentsIds, DatabaseConstants.UnitFullnessStatus.Small.Id);
                var mediumFullnessCount = await GetFullnessCountAsync(contentType, possibleParentsIds, DatabaseConstants.UnitFullnessStatus.Medium.Id);
                var criticalFullnessCount = await GetFullnessCountAsync(contentType, possibleParentsIds, DatabaseConstants.UnitFullnessStatus.Critical.Id);

                var criticalFullnessInLiters = await CalculateTotalLitersAsync(contentType, possibleParentsIds, DatabaseConstants.UnitFullnessStatus.Critical.Id);

                dashboardDto.ContentFullnessLevel.Add(new FullnessLevelDto
                {
                    ContentTypeId = (int)contentType,
                    SmallFullnessCount = smallFullnessCount,
                    MediumFullnessCount = mediumFullnessCount,
                    CriticalFullnessCount = criticalFullnessCount,
                    CriticalFullnessInLiters = criticalFullnessInLiters
                });
            }

            // Units with any alert count
            dashboardDto.UnitsWithAlertsCount = await GetUnitNavigationCount(possibleParentsIds, false);

            return dashboardDto;
        }

        private async Task<int> GetFullnessCountAsync(ContentTypeEnum contentType, List<int> possibleParentsIds, int fullnessStatusId)
        {
            return await _repositoryContext.Units.AsNoTracking()
                        .Where(u => possibleParentsIds.Any(id => id == u.UserId)
                        && u.ContentTypeId == (int)contentType
                        && u.LatestUnitReadings
                            .FirstOrDefault().FullnessStatusId == fullnessStatusId)
                        .CountAsync();
        }

        private async Task<int> CalculateTotalLitersAsync(ContentTypeEnum contentType, List<int> possibleParentsIds, int fullnessStatusId)
        {
            var criticalUnits = await _repositoryContext.Units
                .Include(u => u.LatestUnitReadings)                
                .Include(u => u.UnitModelType)
                .AsNoTracking()
                .Where(u => possibleParentsIds.Any(id => id == u.UserId)
                && u.ContentTypeId == (int)contentType
                && u.LatestUnitReadings
                            .FirstOrDefault().FullnessStatusId == fullnessStatusId)
                .ToListAsync();

            double totalLiters = 0;

            foreach (var unit in criticalUnits)
            {
                var lastReading = unit.LatestUnitReadings.First();
                if (lastReading is not null)
                {
                    var fillLevel = unit.ComputeUnitFillLevelPercentages(lastReading);
                    totalLiters += fillLevel.FillLevelInLiters;
                }
            }

            int roundedLiters = (int)Math.Round(totalLiters, MidpointRounding.AwayFromZero);

            // Return roundedLiters
            return roundedLiters;
        }

        public async Task<List<Unit>> GetUnits(List<int> unitsIds, bool trackChanges = true)
        {
            return await FindAll(trackChanges)
                .Include(u => u.UnitModelType)
                .Include(u => u.ContentType)
                .Include(u => u.LatestUnitReadings)
                  .Where(u => unitsIds.Contains(u.Id))
                  .ToListAsync();
        }

        public async Task<List<Unit>> GetUnits()
        {
            return await FindAll(true)
                .ToListAsync();
        }

        public async Task<bool> CheckIfUnitHaveReadings(int unitId) => (await _repositoryContext.UnitReadings.CountAsync(u => u.UnitId == unitId)) > 1000;

        public async Task LockBin(Unit unit, BinLockInput input, int loggedInUserId)
        {
            unit.LockUntil = input.LockUntil;
            unit.BinLock = input.BinLock;
            unit.LockCommandPending = true;
            unit.UpdateUserId = loggedInUserId;

            await _repositoryContext.SaveChangesAsync();
        }

        public async Task<List<Unit>> GetUnitsForUnlock(bool trackChanges = true)
        => await FindAll(trackChanges).Where(u => !u.Deleted && u.LockUntil < DateTime.UtcNow).ToListAsync();

        public async Task<bool> CheckIfSerialNumbersExist(List<string> serialNumbers)
        {
            return await _repositoryContext.Units
                     .AnyAsync(u => serialNumbers.Contains(u.SerialNumber) && !u.Deleted);
        }
    }
}
