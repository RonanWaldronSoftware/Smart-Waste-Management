using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWaste.Model.JsonModels.Account
{
    public class GetUsersInput
    {
        public bool IncludeInactive { get; set; } = false;
        public bool SimpleVersion { get; set; } = false;
        public string? Search { get; set; } = "";
        public int PerPage { get; set; } = 10;
        public int Page { get; set; } = 1;
        public string SortBy { get; set; } = "CreateDate";
        public string SortByDirection { get; set; } = "desc";
    }
}
