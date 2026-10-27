using SmartWaste.Model.Helpers.Jwt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWaste.Model.JsonModels.Account
{
    public class AccountLoginResponse
    {
        public JwtAuthResult? JwtAuthResult { get; set; } = new();
        public string? OneTimeToken { get; set; }  
    }
}
