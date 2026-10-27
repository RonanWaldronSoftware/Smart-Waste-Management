using System;
using System.Collections.Generic;

namespace SmartWaste.Model.Core;

public partial class Language
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Code { get; set; }

    public DateTime CreateDate { get; set; }

    public string? TranslationKey { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
