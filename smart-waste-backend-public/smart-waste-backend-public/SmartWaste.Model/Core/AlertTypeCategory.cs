using System;
using System.Collections.Generic;

namespace SmartWaste.Model.Core;

public partial class AlertTypeCategory
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? TranslationKey { get; set; }

    public DateTime CreateDate { get; set; }

    public virtual ICollection<AlertType> AlertTypes { get; set; } = new List<AlertType>();
}
