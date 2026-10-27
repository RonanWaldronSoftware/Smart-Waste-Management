using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class Alert
{
    public int Id { get; set; }

    public int? AlertType { get; set; }

    public int? UnitId { get; set; }

    /// <summary>
    /// 0 = new; 1=Being-processed; 2= processed
    /// </summary>
    public DateTime? AlertDateTime { get; set; }

    public short? Status { get; set; }

    public int? UnitReadingId { get; set; }

    /// <summary>
    /// This is used for damage reports only
    /// </summary>
    public string? Comments { get; set; }

    public int? DamageId { get; set; }
}
