using SmartWaste.Model.DataTransferObjects.Account;
using SmartWaste.Model.DataTransferObjects.WebConfiguration;

namespace SmartWaste.Model.DataTransferObjects.Unit
{
    public class UnitDetailsDto : ShortUnitDto
    {
        public string? Location { get; set; }
        public string? SerialNumber { get; set; }
        public int? EmptyLevel { get; set; }
        public int? FullLevel { get; set; }
        public int FillLevelPercentage { get; set; } = 0;
        public double? BatteryVoltage { get; set; }
        public int? Temperature { get; set; }
        public bool BinLock { get; set; }
        public string? SigFoxAltId { get; set; }

        public DateTime? InstallationDate { get; set; }
        public List<UnitImageDto> UnitImages { get; set; } = new();
        public ContentTypeDto ContentType { get; set; }
        public ShortUserDto User { get; set; }
        public ShortUnitModelTypeDto UnitModelType { get; set; }
        public UnitTypeDto UnitType { get; set; } 
        public DateTime? LastReadingCreateDate { get; set; }
        public DateTime? SubscriptionRenewalDate { get; set; }
        public UnitReadingDto UnitReading { get; set; } = new();
        public List<ShortAlertTypeDto> AlertTypes { get; set; } = new();
        public int? DeviceInputTypeId { get; set; }
    }
}
