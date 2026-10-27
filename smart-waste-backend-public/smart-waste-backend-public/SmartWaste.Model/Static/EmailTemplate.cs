using SmartWaste.Model.Core;
using Microsoft.AspNetCore.Hosting;

namespace SmartWaste.Model.Static
{
    public static class EmailTemplate
    {
        public static string PasswordReset(IWebHostEnvironment hostEnvironment, User user, Configuration.Configuration configuration)
        {
            string templateRoot = $"{hostEnvironment.WebRootPath}/Templates/passwordReset.html";

            var html = System.IO.File.ReadAllText($"{templateRoot}")
            .Replace("?Email?", user.Email)
            .Replace("?Link?", $"{configuration.BaseURLFront}forgot-password/{user.OneTimeTokens.FirstOrDefault()?.Token}");

            return html;
        }
    }
}
