using SmartWaste.Model.Core;
using SmartWaste.Model.Extensions;
using SmartWaste.Model.Helpers;
using SmartWaste.Model.JsonModels.Route;
using SmartWaste.Repository.Base;
using SmartWaste.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.Repository.Implementation
{
    public class RouteRepository : RepositoryBase<Route>, IRouteRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public RouteRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task CreateRoute(Route route)
        {
            Create(route);
            await _repositoryContext.SaveChangesAsync();
        }

        public async Task<Route> CreateRoute(string routeName, int customerId, int createUserId)
        {
            Route route = new Route
            {
                Name = routeName,
                UserId = customerId,
                CreateUserId = createUserId
            };

            Create(route);
            await _repositoryContext.SaveChangesAsync();

            return route;
        }

        public async Task<Route?> GetRoute(int routeId, bool trackChanges = true)
        => await FindByCondition(r => r.Id == routeId && !r.Deleted, trackChanges)
                .Include(r => r.RouteUnits).ThenInclude(ru => ru.Unit)
                .FirstOrDefaultAsync();

        public async Task<TableResponse<Route>> GetRoutes(DatatableOptions options, List<int> possibleParentsIds, bool trackChanges = true)
        {
            var routes = FindAll(trackChanges)
              .Include(r => r.RouteUnits)
              .Include(r => r.User)
              .Where(r => !r.Deleted
                          && possibleParentsIds.Contains(r.UserId.Value)
                          && (EF.Functions.Like(r.Name, $"%{options.Search}%")
                              || EF.Functions.Like(r.User.Name, $"%{options.Search}%")));


            options.TotalRecords = await routes.CountAsync();

            return new TableResponse<Route>
            {
                Responses = await routes.OrderByPage(options.SortBy, options.SortByDirection, options.Page, options.PerPage).ToListAsync(),
                Options = options
            };
        }

        public async Task UpdateRouteAndUnits(Route route, RouteUpdateInput input, int updateUserId)
        {

            // Update data
            route.Name = input.Name;
            route.UpdateUserId = updateUserId;
            route.UserId = input.UserId;

            // Delete existing route units
            _repositoryContext.RouteUnits.RemoveRange(route.RouteUnits);

            // Add new route units
            foreach (var newRouteUnit in input.RouteUnits)
            {
                route.RouteUnits.Add(new RouteUnit
                {
                    RouteId = route.Id,
                    RouteIndex = newRouteUnit.RouteIndex,
                    UnitId = newRouteUnit.UnitId,
                });
            }

            // Save changes to the database
            await _repositoryContext.SaveChangesAsync();

        }
    }
}
