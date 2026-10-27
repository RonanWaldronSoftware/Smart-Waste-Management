using SmartWaste.Model.Core;

namespace SmartWaste.Repository.Contracts
{
    public interface IRouteUnitsRepository
    {
        /// <summary>
        /// Create route for unit and save changes
        /// </summary>
        /// <param name="routeUnit"></param>
        /// <returns></returns>
        public Task CreateRouteUnits(RouteUnit routeUnit);

        /// <summary>
        /// Create route for unit
        /// </summary>
        /// <param name="routeId"></param>
        /// <param name="unitId"></param>
        /// <param name="routeIndex"></param>
        public void CreateRouteUnit(int routeId, int unitId, int routeIndex);

        /// <summary>
        /// Delete route units for route
        /// </summary>
        /// <param name="routeId"></param>
        public Task DeleteRouteUnits(int routeId);
    }
}
