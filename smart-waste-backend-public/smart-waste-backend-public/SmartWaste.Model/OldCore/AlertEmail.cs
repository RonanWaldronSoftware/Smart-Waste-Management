using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class AlertEmail
{
    public int Id { get; set; }

    public int? AlertId { get; set; }

    public string? Email { get; set; }

    public string? Subject { get; set; }

    public sbyte? HtmlBody { get; set; }

    public string? Body { get; set; }

    /// <summary>
    /// 0 = created, 1= sent, -1 = failed (reason will be in the comments field)
    /// </summary>
    public int? Status { get; set; }

    public DateTime? InsertDate { get; set; }

    public DateTime? SentDateTime { get; set; }

    public string? Comment { get; set; }
}
