using SmartWaste.Repository.Contracts;

namespace SmartWaste.Repository
{
    public interface IRepositoryManager
    {
        public IAlertTypeCategoryRepository AlertTypeCategory { get; }
        public IAlertTypeRepository AlertType { get; }
        public ICountryRepository Country { get; }
        public IContentTypeRepository ContentType { get; }
        public IDamageTypeRepository DamageType { get; }
        public IDeviceTypeRepository DeviceType { get; }
        public IDeviceInputTypeRepository DeviceInputType { get; }
        public ILanguageRepository Language { get; }
        public IOneTimeTokenRepository OneTimeToken { get; }
        public IRoleRepository Role { get; }
        public IRouteRepository Route { get; }
        public IStatusCategoryRepository StatusCategory { get; }
        public IStatusRepository Status { get; }
        public IUnitImageRepository UnitImage { get; }
        public IUnitModelTypeRepository UnitModelType { get; }
        public IUnitReadingRepository UnitReading { get; }
        public IUnitTypeRepository UnitType { get; }
        public IUnitRepository Unit { get; }
        public IUserRepository User { get; }
        public IRouteUnitsRepository RouteUnit { get; }
        public IUserAlertRepository UserAlert { get; }
        public IDamageRepository Damage { get; }
        public IDamageHistoryRepository DamageHistory { get; }
        public IUserAlertSendRepository UserAlertSend { get; }
        Task SaveAsync();
        void Save();
    }
}
