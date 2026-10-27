namespace SmartWaste.Model.JsonModels.Superfy;

using System.Text.Json.Serialization;

public class AuthResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("message")]
    public AuthMessage Message { get; set; }
}

public class AuthMessage
{
    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonPropertyName("deviceID")]
    public string DeviceID { get; set; }

    [JsonPropertyName("profile")]
    public UserProfile Profile { get; set; }

    [JsonPropertyName("company")]
    public CompanyInfo Company { get; set; }
}

public class UserProfile
{
    [JsonPropertyName("userID")]
    public int UserID { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; }
}

public class CompanyInfo
{
    [JsonPropertyName("companyID")]
    public int CompanyID { get; set; }

    [JsonPropertyName("companyName")]
    public string CompanyName { get; set; }
}
