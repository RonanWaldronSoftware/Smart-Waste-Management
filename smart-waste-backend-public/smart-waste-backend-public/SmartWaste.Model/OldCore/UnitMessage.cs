using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class UnitMessage
{
    public int Id { get; set; }

    public int? UnitId { get; set; }

    public byte[]? Message { get; set; }

    /// <summary>
    /// 0 = created\n1 = sent
    /// </summary>
    public int? Status { get; set; }

    public int? InsertBy { get; set; }

    public DateTime? InsertDate { get; set; }

    public DateTime? SentDate { get; set; }
}
