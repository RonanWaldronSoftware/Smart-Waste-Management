using SmartWaste.Model.Constants;
using SmartWaste.Model.Enums;

namespace SmartWaste.Model.Static
{
    public static class RoleStaticService
    {
        private static readonly Dictionary<int, List<int>> UserRoleMappings = new()
        {
            { DatabaseConstants.Role.Admin.Id, new List<int> { DatabaseConstants.Role.Admin.Id } },
            { DatabaseConstants.Role.Distributor.Id, new List<int> { DatabaseConstants.Role.Admin.Id } },
            { DatabaseConstants.Role.CorporateCustomer.Id, new List<int> { DatabaseConstants.Role.Admin.Id, DatabaseConstants.Role.Distributor.Id } },
            { DatabaseConstants.Role.Customer.Id, new List<int> { DatabaseConstants.Role.Admin.Id, DatabaseConstants.Role.Distributor.Id, DatabaseConstants.Role.CorporateCustomer.Id } },
            { DatabaseConstants.Role.Driver.Id, new List<int> { DatabaseConstants.Role.CorporateCustomer.Id, DatabaseConstants.Role.Customer.Id } },
            { DatabaseConstants.Role.Technician.Id, new List<int> { DatabaseConstants.Role.Admin.Id, DatabaseConstants.Role.Distributor.Id } }
        };

        private static readonly Dictionary<int, List<int>> UserCreateRoleMappings = new()
        {
            { DatabaseConstants.Role.Admin.Id, new List<int> { DatabaseConstants.Role.Admin.Id, DatabaseConstants.Role.Distributor.Id, DatabaseConstants.Role.CorporateCustomer.Id, DatabaseConstants.Role.Customer.Id, DatabaseConstants.Role.Driver.Id, DatabaseConstants.Role.Technician.Id} },
            { DatabaseConstants.Role.Distributor.Id, new List<int> { DatabaseConstants.Role.CorporateCustomer.Id, DatabaseConstants.Role.Customer.Id, DatabaseConstants.Role.Driver.Id, DatabaseConstants.Role.Technician.Id } },
            { DatabaseConstants.Role.CorporateCustomer.Id, new List<int> { DatabaseConstants.Role.Customer.Id, DatabaseConstants.Role.Driver.Id } },
            { DatabaseConstants.Role.Customer.Id, new List<int> { DatabaseConstants.Role.Driver.Id } },
            { DatabaseConstants.Role.Driver.Id, new List<int> {} },
            { DatabaseConstants.Role.Technician.Id, new List<int> {} }
        };

        private static readonly List<int> UnitRolesIds = new()
        {
            DatabaseConstants.Role.Distributor.Id,
            DatabaseConstants.Role.CorporateCustomer.Id,
            DatabaseConstants.Role.Customer.Id,
        };

        private static readonly List<int> RouteRolesIds = new()
        {
            DatabaseConstants.Role.CorporateCustomer.Id,
            DatabaseConstants.Role.Customer.Id,
        };

        public static List<int> GetConfigurationRoles(int roleId)
        {
            return UserCreateRoleMappings.TryGetValue(roleId, out var roles) ? roles : new List<int>();
        }

        public static List<int> GetPossibleRolesByType(PossibleParentTypeEnum type, int? roleId = null)
        {
            if (type == PossibleParentTypeEnum.User)
            {
                if (roleId == null)
                    new List<int>();

                return GetPossibleUserRoles(roleId.Value);
            }

            return type switch
            {
                PossibleParentTypeEnum.Unit => UnitRolesIds,
                PossibleParentTypeEnum.Route => RouteRolesIds,
                PossibleParentTypeEnum.Alert => UnitRolesIds,
                PossibleParentTypeEnum.Damage => UnitRolesIds,
                _ => new List<int>()
            };
        }

        private static List<int> GetPossibleUserRoles(int roleId)
        {
            return UserRoleMappings.TryGetValue(roleId, out var roles) ? roles : new List<int>();
        }

    }
}
