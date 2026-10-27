using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class AlertResult
{
    public int Id { get; set; }

    public int AlertId { get; set; }

    /// <summary>
    /// 1 = email, 2 = sms, 3 = whatsapp, 4 = push
    /// </summary>
    public int? ContactType { get; set; }

    /// <summary>
    /// email, phone number etc..
    /// </summary>
    public string? ContactDetail { get; set; }

    /// <summary>
    /// 0 = success; -1 = failed
    /// </summary>
    public int? Status { get; set; }

    public string? FailReason { get; set; }

    public DateTime? InsertDate { get; set; }
}
