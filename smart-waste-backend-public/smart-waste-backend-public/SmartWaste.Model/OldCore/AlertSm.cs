using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class AlertSm
{
    public int Id { get; set; }

    public int? AlertId { get; set; }

    public string? PhoneNo { get; set; }

    public string? Message { get; set; }

    /// <summary>
    /// 0 = created, 1 = sent, 2 = DLR received
    /// </summary>
    public int? Status { get; set; }

    public int? HttpResponseCode { get; set; }

    public int? ResponseResultCode { get; set; }

    public string? ResponseResultDesc { get; set; }

    public int? SmsCount { get; set; }

    public DateTime? InsertDate { get; set; }

    public DateTime? ResponseDateTime { get; set; }

    public string? Comment { get; set; }
}
