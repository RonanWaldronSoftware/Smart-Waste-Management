using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class AlertSmsResponse
{
    public int Id { get; set; }

    public int AlertSmsId { get; set; }

    public string? MessageId { get; set; }

    public string? Destination { get; set; }

    public int? StatusCode { get; set; }

    public string? StatusDesc { get; set; }

    public string? TransactionId { get; set; }

    public string? Channel { get; set; }

    public string? Time { get; set; }

    public int? DlrStatusCode { get; set; }

    public string? DlrStatusDetails { get; set; }

    /// <summary>
    /// 0 = inserted (after making API call and getting response), 1 = after receiving DLR report
    /// </summary>
    public int Status { get; set; }

    public DateTime? InsertDate { get; set; }

    public DateTime? DlrResponseDateTime { get; set; }
}
