using System;
using System.Collections.Generic;

namespace SmartWaste.Model.Core;

public partial class UnitImage
{
    public int Id { get; set; }

    public int UnitId { get; set; }

    public int UnitCategoryId { get; set; }

    public string Image { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public virtual Unit Unit { get; set; } = null!;

    public virtual UnitCategory UnitCategory { get; set; } = null!;
}
