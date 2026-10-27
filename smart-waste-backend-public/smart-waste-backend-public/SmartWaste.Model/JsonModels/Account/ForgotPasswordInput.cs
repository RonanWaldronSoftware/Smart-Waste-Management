using System.ComponentModel.DataAnnotations;

namespace SmartWaste.Model.JsonModels.Account;

public class ForgotPasswordInput
{
    [Required]
    public string Email { get; set; } = null!;
}

public class ForgotPasswordChangePasswordInput
{
    [Required]
    public string Token { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
}