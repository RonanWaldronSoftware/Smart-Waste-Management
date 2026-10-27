using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class TekelekMessage
{
    public int Id { get; set; }

    public int? UnitId { get; set; }

    public string? Message { get; set; }

    public int? Status { get; set; }

    public int? InsertBy { get; set; }

    public DateTime? InsertDate { get; set; }

    public DateTime? SentDate { get; set; }
}
