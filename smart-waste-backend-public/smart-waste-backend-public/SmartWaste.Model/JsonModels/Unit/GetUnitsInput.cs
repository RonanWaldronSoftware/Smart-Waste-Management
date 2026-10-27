using SmartWaste.Model.Helpers.Filters;
using SmartWaste.Model.Helpers;

namespace SmartWaste.Model.JsonModels.Unit
{
    public class GetUnitsInput
    {
        public DatatableOptions DataTableOptions { get; set; } = new();
        public bool IncludeInactive { get; set; }
        public UnitFilter? Filter { get; set; } = null;
    }
}
