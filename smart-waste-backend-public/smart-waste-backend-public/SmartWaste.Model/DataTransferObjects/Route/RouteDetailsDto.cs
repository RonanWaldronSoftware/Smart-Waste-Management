using SmartWaste.Model.JsonModels.Route;

namespace SmartWaste.Model.DataTransferObjects.Route
{
    public class RouteDetailsDto
    {
        public string Name { get; set; }
        public int UserId { get; set; }
        public List<RouteUnitDetailsDto> RouteUnits { get; set; }
    }
}
