using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SmartWaste.Model.JsonModels.Account;

public class AccountLoginInput
{
    [Required] public string Email { get; set; } = null!;
    [Required] public string Password { get; set; } = null!;
    public string? DeviceToken { get; set; } = string.Empty;
    public int? DeviceType { get; set; }
}