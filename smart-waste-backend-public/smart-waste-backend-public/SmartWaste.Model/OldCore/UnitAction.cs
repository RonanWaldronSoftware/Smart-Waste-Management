using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class UnitAction
{
    public int Id { get; set; }

    public int UnitId { get; set; }

    public string Action { get; set; } = null!;

    public DateTime ActionDateTime { get; set; }

    public int ActionUserId { get; set; }

    public DateTime InsertDateTime { get; set; }

    /// <summary>
    /// 0 - Requested\n1 - Completed
    /// </summary>
    public sbyte Status { get; set; }

    public DateTime? ActionProcessed { get; set; }
}
