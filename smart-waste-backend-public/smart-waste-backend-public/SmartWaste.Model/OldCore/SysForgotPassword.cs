using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class SysForgotPassword
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Uuid { get; set; } = null!;

    public DateTime? RequestDate { get; set; }
}
