using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWaste.Model.Configuration
{
    public class JwtConfiguration
    {
        public const string Section = "JwtConfiguration";
        public string Secret { get; set; }
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
        public string RequiredScope { get; set; }
        public List<string>? DefaultScope { get; set; }
        public int AccessTokenExpiration { get; set; }
        public int RefreshTokenExpiration { get; set; }
    }
}
