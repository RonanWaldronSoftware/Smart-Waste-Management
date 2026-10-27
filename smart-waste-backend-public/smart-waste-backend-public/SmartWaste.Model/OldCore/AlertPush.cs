using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class AlertPush
{
    public int Id { get; set; }

    public int? AlertId { get; set; }

    public string? GcmToken { get; set; }

    public string? Title { get; set; }

    public string? Body { get; set; }

    public int? Status { get; set; }

    public DateTime? InsertDate { get; set; }

    public string? Comment { get; set; }

    public int? HttpResponseCode { get; set; }

    public string? HttpResponseBody { get; set; }

    public DateTime? ResponseDateTime { get; set; }
}
