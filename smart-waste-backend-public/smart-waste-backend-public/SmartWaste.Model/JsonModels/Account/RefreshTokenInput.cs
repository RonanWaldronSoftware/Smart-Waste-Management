using System.ComponentModel.DataAnnotations;

namespace SmartWaste.Model.JsonModels.Account;

public class RefreshTokenInput
{
    [Required]
    public string RefreshToken { get; set; } = null!;
    [Required]
    public string AccessToken { get; set; } = null!;
}