using SmartWaste.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWaste.Model.JsonModels.Account
{
    public class CreateUserInput
    {
        public string? Name { get; set; }

        public string? Email { get; set; }

        public string Password { get; set; } = null!;

        public int ParentId { get; set; }

        public int RoleId { get; set; }

        public int? LanguageId { get; set; }

        public string? Address1 { get; set; }

        public string? Address2 { get; set; }

        public int? BinLevelAlert { get; set; }

        public string? City { get; set; }

        public string? PhoneNumber { get; set; }

        public int? CountryId { get; set; }
    }

    public class UpdateUserInput
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public int? ParentId { get; set; }

        public int RoleId { get; set; }

        public int? LanguageId { get; set; }

        public string? Address1 { get; set; }

        public string? Address2 { get; set; }

        public int? BinLevelAlert { get; set; }

        public string? City { get; set; }

        public string? PhoneNumber { get; set; }

        public int? CountryId { get; set; }
    }
}
