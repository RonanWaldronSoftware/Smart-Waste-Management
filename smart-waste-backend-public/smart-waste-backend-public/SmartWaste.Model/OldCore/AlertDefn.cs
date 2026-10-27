using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class AlertDefn
{
    public int Id { get; set; }

    /// <summary>
    /// This is the customer id of the bins been selected for alerting
    /// </summary>
    public int? CustomerId { get; set; }

    /// <summary>
    /// This is the user to be alerted when an alert is raised
    /// </summary>
    public int? UserId { get; set; }

    public ulong? BinFull { get; set; }

    public ulong? BatteryUvlo { get; set; }

    public ulong? BinEmptiedLastPeriod { get; set; }

    public ulong? BatteryOverTempLo { get; set; }

    public ulong? BinLocked { get; set; }

    public ulong? BinTilted { get; set; }

    public ulong? ServiceDoorOpen { get; set; }

    public ulong? FlapStuckOpen { get; set; }

    public ulong? Damage { get; set; }

    public ulong? NotReporting { get; set; }

    public ulong? Email { get; set; }

    public ulong? Sms { get; set; }

    public ulong? Whatsapp { get; set; }

    public ulong? Push { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? InsertBy { get; set; }

    public DateTime? InsertDate { get; set; }

    public int? ModifiedBy { get; set; }
}
