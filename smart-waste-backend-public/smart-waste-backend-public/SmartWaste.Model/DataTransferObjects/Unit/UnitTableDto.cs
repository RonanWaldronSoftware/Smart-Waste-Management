using SmartWaste.Model.DataTransferObjects.WebConfiguration;

namespace SmartWaste.Model.DataTransferObjects.Unit
{
    public class UnitTableDto
    {
        public int Id { get; set; }
        public string? SerialNumber { get; set; }
        public string? Location { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public ContentTypeDto ContentType { get; set; }
        public int? EmptyLevel { get; set; }
        public int? FullLevel { get; set; }
        public int FillLevelPercentage { get; set; } = 0;
        public string CustomerName { get; set; }
        public double? BatteryVoltage { get; set; }
        public int? Temperature { get; set; }
        public DateTime? LastUnitReadingCreateDate { get; set; }
        public DateTime? SubscriptionRenewalDate { get; set; }
        /// <summary>
        /// 11-active, 9-inactive
        /// </summary>
        public int? StatusId { get; set; }
        public List<ShortAlertTypeDto> AlertTypes { get; set; } = new();
    }
}
