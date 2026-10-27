using System;
using System.Collections.Generic;

namespace SmartWaste.Model.Core;

public partial class DamageType
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? TranslationKey { get; set; }

    public virtual ICollection<Damage> Damages { get; set; } = new List<Damage>();
}
