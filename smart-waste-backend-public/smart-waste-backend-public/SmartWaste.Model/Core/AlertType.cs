using System;
using System.Collections.Generic;

namespace SmartWaste.Model.Core;

public partial class AlertType
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? TranslationKey { get; set; }

    public int? AlertTypeCategoryId { get; set; }

    public DateTime CreateDate { get; set; }

    public virtual AlertTypeCategory? AlertTypeCategory { get; set; }

    public virtual ICollection<UserAlert> UserAlerts { get; set; } = new List<UserAlert>();
}
