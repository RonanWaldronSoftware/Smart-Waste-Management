using SmartWaste.Model.Core;
using SmartWaste.Model.Helpers;

namespace SmartWaste.Model.JsonModels.Unit
{
    public class UpdateUnitInput : BaseUnitInput
    {
        public int Id { get; set; }
        public string? SerialNumber { get; set; }
    }

    public class CreateUnitInput : BaseUnitInput
    {
        public List<string> SerialNumbers { get; set; } = new();
    }

    public class BaseUnitInput
    {
        public int? UserId { get; set; }
        public int? UnitModelTypeId { get; set; }
        public string? Location { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int? UnitTypeId { get; set; }
        public int? ContentTypeId { get; set; }
        public int? EmptyLevel { get; set; }
        public int? FullLevel { get; set; }
        public DateTime? SubscriptionRenewalDate { get; set; }
        public string? SigFoxAltId { get; set; }
        public int? DeviceInputTypeId { get; set; }
    }

    public class UnitInstallationInput()
    {
        public string SerialNumber { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Location { get; set; }
        public FileData ImageData { get; set; }
    }
}
