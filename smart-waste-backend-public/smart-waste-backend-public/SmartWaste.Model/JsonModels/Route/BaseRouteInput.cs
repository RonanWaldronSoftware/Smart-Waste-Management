namespace SmartWaste.Model.JsonModels.Route
{
    public class BaseRouteInput
    {
        public string Name { get; set; }

        public List<RouteUnitDto> RouteUnits { get; set; }
    }

    public class CreateRouteInput : BaseRouteInput
    {
        public string Name { get; set; }
        public int UserId { get; set; }
    }

    public class RouteUpdateInput : BaseRouteInput
    {
        public int Id { get; set; }
        public int UserId { get; set; }
    }

    public class RouteUnitDto
    {
        public int UnitId { get; set; }
        public int? RouteIndex { get; set; }
    }

    public class RouteUnitDetailsDto : RouteUnitDto
    {
        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }
    }
}
