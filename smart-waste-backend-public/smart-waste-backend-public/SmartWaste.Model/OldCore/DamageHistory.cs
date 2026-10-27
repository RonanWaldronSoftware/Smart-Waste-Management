using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class DamageHistory
{
    public int Id { get; set; }

    public int? DamageId { get; set; }

    /// <summary>
    /// status references ref_damage_status = opened, assigned, closed
    /// </summary>
    public int? Status { get; set; }

    public string? Comment { get; set; }

    public int? ActionUserId { get; set; }

    public DateTime? ActionDate { get; set; }

    public int? AssignedTo { get; set; }

    public byte[]? Image { get; set; }

    public virtual Damage? Damage { get; set; }
}
