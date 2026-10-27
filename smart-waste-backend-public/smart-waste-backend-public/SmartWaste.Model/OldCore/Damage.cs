using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class Damage
{
    public int Id { get; set; }

    public int? DamageTypeId { get; set; }

    public int? UnitId { get; set; }

    public int? Status { get; set; }

    public DateTime? InsertDate { get; set; }

    public virtual ICollection<DamageHistory> DamageHistories { get; set; } = new List<DamageHistory>();
}
