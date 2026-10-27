using SmartWaste.Model.Core;
using SmartWaste.Model.Helpers;
using SmartWaste.Model.JsonModels.Route;

namespace SmartWaste.Repository.Contracts
{
    public interface IRouteRepository
    {
        /// <summary>
        /// Create route
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        public Task CreateRoute(Route route);
        /// <summary>
        /// Get route
        /// </summary>
        /// <param name="routeId"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        public Task<Route?> GetRoute(int routeId, bool trackChanges = true);
        /// Create route
        /// </summary>
        /// <param name="routeName"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public Task<Route> CreateRoute(string routeName, int customerId, int createUserId);
        /// <summary>
        /// Get routes
        /// </summary>
        /// <param name="options"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        public Task<TableResponse<Route>> GetRoutes(DatatableOptions options, List<int> possibleParentsIds, bool trackChanges = true);
        /// <summary>
        /// Update route and route units
        /// </summary>
        /// <param name="input"></param>
        /// <param name="updateUserId"></param>
        /// <returns></returns>
        Task UpdateRouteAndUnits(Route route, RouteUpdateInput input, int updateUserId);
    }
}
