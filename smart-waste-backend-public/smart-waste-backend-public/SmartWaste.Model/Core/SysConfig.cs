using System;
using System.Collections.Generic;

namespace SmartWaste.Model.Core;

public partial class SysConfig
{
    public string? Name { get; set; }

    public string? Value { get; set; }

    public int? UpdateUserId { get; set; }

    public DateTime? UpdateDate { get; set; }

    public int Id { get; set; }

    public DateTime CreateDate { get; set; }

    public string? TranslationKey { get; set; }

    public virtual User? UpdateUser { get; set; }
}
