namespace SmartWaste.Model.JsonModels
{
    public class ExportInput
    {
        public int? UnitId { get; set; }
        public int ExportFormatId { get; set; }
        public int ExportDataTypeId { get; set; }
        public List<int> DataIds { get; set; } = new();
        public DateTime? From {  get; set; }
        public DateTime? To {  get; set; }
        public int TimeZoneOffset { get; set; } = 0;
    }
}
