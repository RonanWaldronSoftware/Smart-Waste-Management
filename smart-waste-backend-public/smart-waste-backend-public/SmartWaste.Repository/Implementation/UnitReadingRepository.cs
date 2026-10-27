using SmartWaste.Model.Core;
using SmartWaste.Model.Extensions;
using SmartWaste.Model.Helpers;
using SmartWaste.Model.JsonModels.Unit;
using SmartWaste.Model.OldCore;
using SmartWaste.Repository.Base;
using SmartWaste.Repository.Contracts;
using Microsoft.EntityFrameworkCore;
using Debug = System.Diagnostics.Debug;
using UnitReading = SmartWaste.Model.Core.UnitReading;

namespace SmartWaste.Repository.Implementation
{
    public class UnitReadingRepository : RepositoryBase<UnitReading>, IUnitReadingRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public UnitReadingRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task<UnitReading> CreateReading(UnitReading unitReading)
        {
            Create(unitReading);
            await _repositoryContext.SaveChangesAsync();

            return unitReading;
        }

        public async Task CreateReadings(List<UnitReading> unitReadings)
        {
            CreateList(unitReadings);
            await _repositoryContext.SaveChangesAsync();
        }

        public async Task<UnitReading?> GetLastUnitReading(int unitId)
            => await _repositoryContext.UnitReadings.OrderByDescending(r => r.CreateDate).FirstOrDefaultAsync(r => r.UnitId == unitId);

        public async Task<LatestUnitReading?> GetLatestUnitReadingFromLatestUnitReading(int unitId, bool trackChanges = true)
        {
            if (trackChanges)
                return await _repositoryContext.LatestUnitReadings.FirstOrDefaultAsync(u => u.UnitId == unitId);
            else
            {
                return await _repositoryContext.LatestUnitReadings
                                               .AsNoTracking()
                                               .FirstOrDefaultAsync(u => u.UnitId == unitId);
            }
        }


        public async Task AddNewLatestReading(LatestUnitReading latestUnitReading)
        {
            await _repositoryContext.LatestUnitReadings.AddAsync(latestUnitReading);
            await _repositoryContext.SaveChangesAsync();
        }

        public async Task<List<UnitReading>> GetUnassignedReadings(List<string> serialNumbers, bool trackChanges = true)
        => await FindAll(trackChanges).Where(ur => ur.UnitId == null && serialNumbers.Any(serialNumber => ur.SerialNumber.Equals(serialNumber))).ToListAsync();

        public async Task<UnitReading?> GetUnitReading(int id, bool trackChanges = true)
        => await FindByCondition(ur => ur.Id == id, trackChanges)
            .Include(ur => ur.Unit).ThenInclude(u => u.User)
            .FirstOrDefaultAsync();

        public async Task<TableResponse<UnitReading>> GetUnitReadings(UnitReadingDatatableOptions options, bool trackChanges = true)
        {
            var query = FindAll(trackChanges)
                            .Include(ur => ur.Unit)
                               .Where(ur => options.UnitId == ur.UnitId && (options.ForLastDays == null || ur.CreateDate >= DateTime.UtcNow.AddDays(-options.ForLastDays.Value)));

            options.TotalRecords = await query.CountAsync();

            var response = await query.OrderByPage(options.SortBy, options.SortByDirection, options.Page, options.PerPage).ToListAsync();

            if (options.ForLastDays is not null)
            {
                var startDate = DateTime.UtcNow.AddDays(-options.ForLastDays.Value);

                var graphResponse = new List<UnitReading>();

                for (DateTime currentStart = startDate; currentStart < DateTime.UtcNow.AddHours(-3); currentStart = currentStart.AddHours(3))
                {
                    DateTime currentEnd = currentStart.AddHours(3);

                    // Get interval of 3 hours
                    var intervalReadings = await query.Where(ur => ur.CreateDate >= currentStart && ur.CreateDate < currentEnd).ToListAsync();

                    if (intervalReadings.Any())
                    {
                        var avgBattery = intervalReadings.Average(r => r.BatteryVoltage);
                        var avgBinLevel = intervalReadings.Average(r => r.BinLevel);
                        var avgTemperature = intervalReadings.Average(r => r.Temperature);
                        var avgRsrq = intervalReadings.Average(r => r.Rsrq);

                        var graphUnitReading = new UnitReading
                        {
                            BatteryVoltage = (int?)avgBattery,
                            Temperature = (int?)avgTemperature,
                            BinLevel = (int?)avgBinLevel,
                            Rsrq = avgRsrq,
                            ReadingDate = currentStart,
                            Unit = intervalReadings.First().Unit,
                            UnitId = intervalReadings.First().UnitId,
                        };
                        graphResponse.Add(graphUnitReading);
                    }
                    else
                    {
                        graphResponse.Add(new UnitReading { ReadingDate = currentStart });
                    }
                }
                // Set graph data to response
                response = graphResponse;
            }
            return new TableResponse<UnitReading>
            {
                Responses = response,
                Options = options
            };
        }

        public async Task<List<UnitReading>> GetUnitReadings(List<int> unitReadingsIds, DateTime? from, DateTime? to, int? unitId, bool trackChanges = true)
        {
            var query = FindAll(trackChanges).Include(u => u.Unit);

            if (unitReadingsIds != null && unitReadingsIds.Any())
                return await query.Where(u => unitReadingsIds.Contains(u.Id)).ToListAsync();

            if (!unitId.HasValue)
                return new List<UnitReading>();

            var filtered = query.Where(u => u.UnitId == unitId);

            if (from.HasValue)
                filtered = filtered.Where(u => u.CreateDate >= from.Value.Date);
            if (to.HasValue)
                filtered = filtered.Where(u => u.CreateDate < to.Value.Date.AddDays(1));

            return await filtered.ToListAsync();
        }
    }
}
