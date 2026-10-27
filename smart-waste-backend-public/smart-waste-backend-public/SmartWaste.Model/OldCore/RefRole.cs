using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class RefRole
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
