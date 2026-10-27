using System;
using System.Collections.Generic;

namespace SmartWaste.Model.Core;

public partial class Unit
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? SerialNumber { get; set; }

    public int? UnitModelTypeId { get; set; }

    public string? Location { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public int? UnitTypeId { get; set; }

    public int? ContentTypeId { get; set; }

    public int? EmptyLevel { get; set; }

    public int? FullLevel { get; set; }

    public int? StatusId { get; set; }

    public string? FirmWare { get; set; }

    public int? TimeDifference { get; set; }

    public bool UnitJustOn { get; set; }

    public bool RegularPeriodicReporting { get; set; }

    public bool NbloTissue { get; set; }

    public int? HardwareRevision { get; set; }

    public int? CreateUserId { get; set; }

    public DateTime CreateDate { get; set; }

    public int? UpdateUserId { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? SigFoxAltId { get; set; }

    public DateTime? SubscriptionRenewalDate { get; set; }

    public DateTime? InstallationDate { get; set; }

    public bool Deleted { get; set; }

    public bool BinLock { get; set; }

    public DateTime? LockUntil { get; set; }

    public bool LockCommandPending { get; set; }

    public int? DeviceInputTypeId { get; set; }

    public string? SuperfyGuid { get; set; }

    public virtual ContentType? ContentType { get; set; }

    public virtual User? CreateUser { get; set; }

    public virtual ICollection<Damage> Damages { get; set; } = new List<Damage>();

    public virtual DeviceInputType? DeviceInputType { get; set; }

    public virtual ICollection<LatestUnitReading> LatestUnitReadings { get; set; } = new List<LatestUnitReading>();

    public virtual ICollection<RouteUnit> RouteUnits { get; set; } = new List<RouteUnit>();

    public virtual Status? Status { get; set; }

    public virtual ICollection<UnitImage> UnitImages { get; set; } = new List<UnitImage>();

    public virtual UnitModelType? UnitModelType { get; set; }

    public virtual ICollection<UnitReading> UnitReadings { get; set; } = new List<UnitReading>();

    public virtual UnitType? UnitType { get; set; }

    public virtual User? UpdateUser { get; set; }

    public virtual User? User { get; set; }

    public virtual ICollection<UserAlertSend> UserAlertSends { get; set; } = new List<UserAlertSend>();
}
