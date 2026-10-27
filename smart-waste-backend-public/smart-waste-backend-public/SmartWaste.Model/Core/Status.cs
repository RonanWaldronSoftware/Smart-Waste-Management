using System;
using System.Collections.Generic;

namespace SmartWaste.Model.Core;

public partial class Status
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? StatusCategoryId { get; set; }

    public string? TranslationKey { get; set; }

    public virtual ICollection<DamageHistory> DamageHistories { get; set; } = new List<DamageHistory>();

    public virtual ICollection<Damage> Damages { get; set; } = new List<Damage>();

    public virtual ICollection<LatestUnitReading> LatestUnitReadings { get; set; } = new List<LatestUnitReading>();

    public virtual StatusCategory? StatusCategory { get; set; }

    public virtual ICollection<UnitReading> UnitReadings { get; set; } = new List<UnitReading>();

    public virtual ICollection<Unit> Units { get; set; } = new List<Unit>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
