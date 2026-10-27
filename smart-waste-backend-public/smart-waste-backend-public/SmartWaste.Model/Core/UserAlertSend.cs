using System;
using System.Collections.Generic;

namespace SmartWaste.Model.Core;

public partial class UserAlertSend
{
    public int Id { get; set; }

    public int? UserAlertId { get; set; }

    public int? UnitId { get; set; }

    public DateTime? CreateDate { get; set; }

    public virtual Unit? Unit { get; set; }

    public virtual UserAlert? UserAlert { get; set; }
}
