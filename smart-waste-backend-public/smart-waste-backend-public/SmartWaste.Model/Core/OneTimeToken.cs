using System;
using System.Collections.Generic;

namespace SmartWaste.Model.Core;

public partial class OneTimeToken
{
    public int UserId { get; set; }

    public Guid? Token { get; set; }

    public int Id { get; set; }

    public DateTime CreateDate { get; set; }

    public virtual User User { get; set; } = null!;
}
