using SmartWaste.Model.Core;
using SmartWaste.Repository.Base;
using SmartWaste.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.Repository.Implementation
{
    public class RouteUnitRepository : RepositoryBase<RouteUnit>, IRouteUnitsRepository
    {
        private readonly RepositoryContext _repositoryContext;
        public RouteUnitRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }


        public async Task CreateRouteUnits(RouteUnit routeUnit)
        {
            Create(routeUnit);
            await _repositoryContext.SaveChangesAsync();
        }

        public void CreateRouteUnit(int routeId, int unitId, int routeIndex)
        {
            var routeUnit = new RouteUnit
            {
                RouteId = routeId,
                UnitId = unitId,
                RouteIndex = routeIndex
            };

            Create(routeUnit);
        }

        public async Task DeleteRouteUnits(int routeId)
        {
            List<RouteUnit> routeUnits = await _repositoryContext.RouteUnits.Where(ru => ru.RouteId.Equals(routeId)).ToListAsync();

            DeleteList(routeUnits);

            await _repositoryContext.SaveChangesAsync();
        }
    }
}
