using System;
using System.Collections.Generic;

namespace SmartWaste.Model.Core;

public partial class ContentType
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? TranslationKey { get; set; }

    public virtual ICollection<Unit> Units { get; set; } = new List<Unit>();
}
