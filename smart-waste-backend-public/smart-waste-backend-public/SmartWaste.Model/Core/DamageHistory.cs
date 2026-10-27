using System;
using System.Collections.Generic;

namespace SmartWaste.Model.Core;

public partial class DamageHistory
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? AssignedUserId { get; set; }

    public int? DamageId { get; set; }

    public int? StatusId { get; set; }

    public string? Comment { get; set; }

    public string? Image { get; set; }

    public DateTime CreateDate { get; set; }

    public virtual User? AssignedUser { get; set; }

    public virtual Damage? Damage { get; set; }

    public virtual Status? Status { get; set; }

    public virtual User? User { get; set; }
}
