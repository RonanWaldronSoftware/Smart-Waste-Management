using System;
using System.Collections.Generic;

namespace SmartWaste.Model.Core;

public partial class UserAlert
{
    public int Id { get; set; }

    public int? AlertTypeId { get; set; }

    public int? UserId { get; set; }

    public int? CustomerUserId { get; set; }

    public DateTime CreateDate { get; set; }

    public virtual AlertType? AlertType { get; set; }

    public virtual User? CustomerUser { get; set; }

    public virtual User? User { get; set; }

    public virtual ICollection<UserAlertSend> UserAlertSends { get; set; } = new List<UserAlertSend>();
}
