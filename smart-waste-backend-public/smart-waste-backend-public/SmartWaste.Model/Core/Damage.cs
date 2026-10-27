using System;
using System.Collections.Generic;

namespace SmartWaste.Model.Core;

public partial class Damage
{
    public int Id { get; set; }

    public int? DamageTypeId { get; set; }

    public int? UnitId { get; set; }

    public int? StatusId { get; set; }

    public DateTime CreateDate { get; set; }

    public virtual ICollection<DamageHistory> DamageHistories { get; set; } = new List<DamageHistory>();

    public virtual DamageType? DamageType { get; set; }

    public virtual Status? Status { get; set; }

    public virtual Unit? Unit { get; set; }
}
