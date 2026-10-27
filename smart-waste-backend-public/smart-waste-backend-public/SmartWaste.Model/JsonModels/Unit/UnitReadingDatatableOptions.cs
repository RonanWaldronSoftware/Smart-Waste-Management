using SmartWaste.Model.Helpers;

namespace SmartWaste.Model.JsonModels.Unit
{
    public class UnitReadingDatatableOptions : DatatableOptions
    {
        public int UnitId { get; set; }
        public int? ForLastDays { get; set; }
    }
}
