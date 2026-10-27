using SmartWaste.Model.DataTransferObjects.WebConfiguration;

namespace SmartWaste.Model.JsonModels.Account
{
    public class WebConfigurationResponse
    {
        public List<Model.DataTransferObjects.WebConfiguration.AlertTypeDto> AlertTypes { get; set; } = [];
        public List<AlertTypeCategoryDto> AlertTypeCategories { get; set; } = [];
        public List<ContentTypeDto> ContentTypes { get; set; } = [];
        public List<DamageTypeDto> DamageTypes { get; set; } = [];
        public List<RoleDto> Roles { get; set; } = [];
        public List<StatusDto> Status { get; set; } = [];
        public List<StatusCategoryDto> StatusCategories { get; set; } = [];
        public List<UnitModelTypeDto> UnitModelTypes { get; set; } = [];
        public List<UnitTypeDto> UnitTypes { get; set; } = [];
        public List<CountryDto> Countries { get; set; } = [];
        public List<LanguageDto> Languages { get; set; } = [];
        public List<DeviceInputTypeDto> DeviceInputTypes { get; set; } = [];
    }

    public class StaticConfigurationResponse
    {
        public List<Config> Configurations { get; set; } = [];
    }

    public class Config
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
