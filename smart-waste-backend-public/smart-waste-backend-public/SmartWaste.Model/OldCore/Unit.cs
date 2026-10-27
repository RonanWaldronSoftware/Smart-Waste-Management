using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

/// <summary>
/// These are the units on the bins - this table stores unit and bin specific data
/// </summary>
public partial class Unit
{
    public int Id { get; set; }

    public int? OwnerId { get; set; }

    public string? SerialNo { get; set; }

    /// <summary>
    /// 1= PelBin Sensor; 2 = Tekelek Sensor
    /// </summary>
    public int? DeviceType { get; set; }

    public string? Location { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public int? BinType { get; set; }

    public int? ContentType { get; set; }

    /// <summary>
    ///  = 0 - use emptyLevel of unit (this table)\n = 1 - use emptyLevel of binType\n
    /// </summary>
    public int? UseBinTypeLevel { get; set; }

    /// <summary>
    /// Once bin level is over this, it should be flagged for emptying
    /// 
    /// </summary>
    public int? EmptyLevel { get; set; }

    public int? FullLevel { get; set; }

    public DateTime? LastActivity { get; set; }

    /// <summary>
    /// 0 = registered, 1 = installed, -1 = deactivate
    /// </summary>
    public int? Status { get; set; }

    public string? Firmware { get; set; }

    public int? TimeDiff { get; set; }

    public ulong? BinJustOn { get; set; }

    public ulong? RegularPeriodicReporting { get; set; }

    public ulong? NbIoTissue { get; set; }

    public int? HardwareRevision { get; set; }

    public int? InsertBy { get; set; }

    public DateTime? InsertDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? SigfoxAltId { get; set; }

    public DateTime? SubscriptionRenewalDate { get; set; }

    public DateTime? InstallationDate { get; set; }

    public virtual RefBinType? BinTypeNavigation { get; set; }

    public virtual RefContentType? ContentTypeNavigation { get; set; }

    public virtual RefDeviceType? DeviceTypeNavigation { get; set; }

    public virtual User? Owner { get; set; }

    public virtual ICollection<RouteUnit> RouteUnits { get; set; } = new List<RouteUnit>();
}
