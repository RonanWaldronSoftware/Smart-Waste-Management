using AutoMapper;
using SmartWaste.Model.Constants;
using SmartWaste.Model.Core;
using SmartWaste.Model.DataTransferObjects.WebConfiguration;
using SmartWaste.Model.Enums;
using SmartWaste.Model.JsonModels.Account;
using SmartWaste.Model.Resources;
using SmartWaste.Model.Static;
using SmartWaste.Repository;
using SmartWaste.Service.Contracts;
using System.Data;

namespace SmartWaste.Service.Implementations
{
    public class ConfigurationService : IConfigurationService
    {

        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private LocalizerService _localizer;

        public ConfigurationService(IRepositoryManager repositoryManager, IMapper mapper, LocalizerService localizer) 
        { 
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<WebConfigurationResponse> GetConfiguration(int languageId, int userRoleId)
        {
            WebConfigurationResponse staticConfiguration = new WebConfigurationResponse();

            Language language = await _repositoryManager.Language.GetLanguage(languageId);

            // Get AlertTypes
            staticConfiguration.AlertTypes = await FetchAndTranslate<AlertType, AlertTypeDto>
                (_repositoryManager.AlertType.GetAlertTypes, alertType => alertType.TranslationKey, language.Code);

            // Battery Over Temperature is a SmartWaste admin-only alert: hide it from the catalog
            // for every non-admin role so it never appears as a selectable option in any client.
            if (userRoleId != DatabaseConstants.Role.Admin.Id)
                staticConfiguration.AlertTypes = staticConfiguration.AlertTypes
                    .Where(alertType => alertType.Id != (int)AlertTypeEnum.BatteryOverTemperature).ToList();

            // Get AlertTypeCategories
            staticConfiguration.AlertTypeCategories = await FetchAndTranslate<AlertTypeCategory, AlertTypeCategoryDto>
                (_repositoryManager.AlertTypeCategory.GetAlertTypeCategories, alertTypeCategory => alertTypeCategory.TranslationKey, language.Code);

            // Get ContentTypes
            staticConfiguration.ContentTypes = await FetchAndTranslate<ContentType, ContentTypeDto>
                (_repositoryManager.ContentType.GetContentTypes, contentType => contentType.TranslationKey, language.Code);

            // Get DamageTypes
            staticConfiguration.DamageTypes = await FetchAndTranslate<DamageType, DamageTypeDto>
                (_repositoryManager.DamageType.GetDamageTypes, damageType => damageType.TranslationKey, language.Code);

            // Get Roles 
            staticConfiguration.Roles = await FetchAndTranslate<Role, RoleDto>
                (_repositoryManager.Role.GetRoles, role => role.TranslationKey, language.Code);
            staticConfiguration.Roles = await AdjustRoles(userRoleId, staticConfiguration.Roles);

            // Get Status
            staticConfiguration.Status = await FetchAndTranslate<Status, StatusDto>
                (_repositoryManager.Status.GetStatuses, status => status.TranslationKey, language.Code);

            // Get StatusCategories
            staticConfiguration.StatusCategories = _mapper.Map<List<StatusCategoryDto>>(await _repositoryManager.StatusCategory.GetStatusCategories());

            // Get UnitModelTypes
            staticConfiguration.UnitModelTypes = await FetchAndTranslate<UnitModelType, UnitModelTypeDto>
                (_repositoryManager.UnitModelType.GetUnitModelTypes, unitModelType => unitModelType.TranslationKey, language.Code);

            // Get UnitTypes
            staticConfiguration.UnitTypes = await FetchAndTranslate<UnitType, UnitTypeDto>
                (_repositoryManager.UnitType.GetUnitTypes, unitType => unitType.TranslationKey, language.Code);

            // Get languages
            staticConfiguration.Languages = await FetchAndTranslate<Language, LanguageDto>
                (_repositoryManager.Language.GetLanguages, thisLanguage => thisLanguage.TranslationKey, language.Code);

            // Get countries
            staticConfiguration.Countries = await FetchAndTranslate<Country, CountryDto>
                (_repositoryManager.Country.GetCountries, country => country.TranslationKey, language.Code);

            // Get device input types
            staticConfiguration.DeviceInputTypes = await FetchAndTranslate<DeviceInputType, DeviceInputTypeDto>
                (_repositoryManager.DeviceInputType.GetDeviceInputTypes, deviceInputType => deviceInputType.TranslationKey, language.Code);

            return staticConfiguration;
        }


        #region private
        private async Task<List<TDto>> FetchAndTranslate<TEntity, TDto>(Func<Task<List<TEntity>>> fetchData, Func<TEntity, string> getTranslationKey, string languageCode)
        {
            // Get data for configuration
            List<TEntity> entities = await fetchData();

            // Go thru all items in the list and localize the name
            foreach (var entity in entities)
            {
                string translationKey = getTranslationKey(entity);
                if (!string.IsNullOrEmpty(translationKey))
                {
                    entity.GetType().GetProperty("Name").SetValue(entity, _localizer.GetLocalString(translationKey, languageCode));
                }
            }
            return _mapper.Map<List<TDto>>(entities);
        }

        private async Task<List<RoleDto>> AdjustRoles(int userRoleId, List<RoleDto> roles)
        {
            // Remove all roles that user can't create
            return roles.Where(r => RoleStaticService.GetConfigurationRoles(userRoleId).Contains(r.Id)).ToList();
        }
        #endregion
    }
}
