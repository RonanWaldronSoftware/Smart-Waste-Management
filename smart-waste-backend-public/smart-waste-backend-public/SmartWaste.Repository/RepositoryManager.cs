using SmartWaste.Model.Core;
using SmartWaste.Repository.Contracts;
using SmartWaste.Repository.Implementation;

namespace SmartWaste.Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _repositoryContext;

        private readonly Lazy<IAlertTypeCategoryRepository> _alertTypeCategoryRepository;
        private readonly Lazy<IAlertTypeRepository> _alertTypeRepository;
        private readonly Lazy<ICountryRepository> _countryRepository;
        private readonly Lazy<IContentTypeRepository> _contentTypeRepository;
        private readonly Lazy<IDamageTypeRepository> _damageTypeRepository;
        private readonly Lazy<IDeviceTypeRepository> _deviceTypeRepository;
        private readonly Lazy<IDeviceInputTypeRepository> _deviceInputTypeRepository;
        private readonly Lazy<ILanguageRepository> _languageRepository;
        private readonly Lazy<IOneTimeTokenRepository> _oneTimeTokenRepository;
        private readonly Lazy<IRoleRepository> _roleRepository;
        private readonly Lazy<IRouteRepository> _routeRepository;
        private readonly Lazy<IStatusCategoryRepository> _statusCategoryRepository;
        private readonly Lazy<IStatusRepository> _statusRepository;
        private readonly Lazy<IUnitImageRepository> _unitImageRepository;
        private readonly Lazy<IUnitModelTypeRepository> _unitModelTypeRepository;
        private readonly Lazy<IUnitReadingRepository> _unitReadingRepository;
        private readonly Lazy<IUnitRepository> _unitRepository;
        private readonly Lazy<IUnitTypeRepository> _unitTypeRepository;
        private readonly Lazy<IUserRepository> _userRepository;
        private readonly Lazy<IRouteUnitsRepository> _routeUnitRepository;
        private readonly Lazy<IUserAlertRepository> _userAlertRepository;
        private readonly Lazy<IDamageRepository> _damageRepository;
        private readonly Lazy<IDamageHistoryRepository> _damageHistoryRepository;
        private readonly Lazy<IUserAlertSendRepository> _userAlertSendRepository;

        public RepositoryManager(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;

            _alertTypeCategoryRepository = new Lazy<IAlertTypeCategoryRepository>(() => new AlertTypeCategoryRepository(repositoryContext));
            _alertTypeRepository = new Lazy<IAlertTypeRepository>(() => new AlertTypeRepository(repositoryContext));
            _countryRepository = new Lazy<ICountryRepository>(() => new CountryRepository(repositoryContext));
            _contentTypeRepository = new Lazy<IContentTypeRepository>(() => new ContentTypeRepository(repositoryContext));
            _damageTypeRepository = new Lazy<IDamageTypeRepository>(() => new DamageTypeRepository(repositoryContext));
            _deviceTypeRepository = new Lazy<IDeviceTypeRepository>(() => new DeviceTypeRepository(repositoryContext));
            _deviceInputTypeRepository = new Lazy<IDeviceInputTypeRepository>(() => new DeviceInputTypeRepository(repositoryContext));
            _languageRepository = new Lazy<ILanguageRepository>(() => new LanguageRepository(repositoryContext));
            _oneTimeTokenRepository = new Lazy<IOneTimeTokenRepository>(() => new OneTimeTokenRepository(repositoryContext));
            _roleRepository = new Lazy<IRoleRepository>(() => new RoleRepository(repositoryContext));
            _routeRepository = new Lazy<IRouteRepository>(() => new RouteRepository(repositoryContext));
            _statusCategoryRepository = new Lazy<IStatusCategoryRepository>(() => new StatusCategoryRepository(repositoryContext));
            _statusRepository = new Lazy<IStatusRepository>(() => new StatusRepository(repositoryContext));
            _unitImageRepository = new Lazy<IUnitImageRepository>(() => new UnitImageRepository(repositoryContext));
            _unitModelTypeRepository = new Lazy<IUnitModelTypeRepository>(() => new UnitModelTypeRepository(repositoryContext));
            _unitReadingRepository = new Lazy<IUnitReadingRepository>(() => new UnitReadingRepository(repositoryContext));
            _unitRepository = new Lazy<IUnitRepository>(() => new UnitRepository(repositoryContext));
            _unitTypeRepository = new Lazy<IUnitTypeRepository>(() => new UnitTypeRepository(repositoryContext));
            _userRepository = new Lazy<IUserRepository>(() => new UserRepository(repositoryContext));
            _routeUnitRepository = new Lazy<IRouteUnitsRepository>(() => new RouteUnitRepository(repositoryContext));
            _userAlertRepository = new Lazy<IUserAlertRepository>(() => new UserAlertRepository(repositoryContext));
            _damageRepository = new Lazy<IDamageRepository>(() => new DamageRepository(repositoryContext));
            _damageHistoryRepository = new Lazy<IDamageHistoryRepository>(() => new DamageHistoryRepository(repositoryContext));
            _userAlertSendRepository = new Lazy<IUserAlertSendRepository>(() => new UserAlertSendRepository(repositoryContext));
        }

        public IAlertTypeCategoryRepository AlertTypeCategory => _alertTypeCategoryRepository.Value;

        public IAlertTypeRepository AlertType => _alertTypeRepository.Value;

        public ICountryRepository Country => _countryRepository.Value;

        public IContentTypeRepository ContentType => _contentTypeRepository.Value;

        public IDamageTypeRepository DamageType => _damageTypeRepository.Value;

        public IDeviceTypeRepository DeviceType => _deviceTypeRepository.Value;

        public IDeviceInputTypeRepository DeviceInputType => _deviceInputTypeRepository.Value;

        public ILanguageRepository Language => _languageRepository.Value;

        public IOneTimeTokenRepository OneTimeToken => _oneTimeTokenRepository.Value;

        public IRoleRepository Role => _roleRepository.Value;

        public IStatusCategoryRepository StatusCategory => _statusCategoryRepository.Value;

        public IStatusRepository Status => _statusRepository.Value;

        public IUnitImageRepository UnitImage => _unitImageRepository.Value;

        public IUnitModelTypeRepository UnitModelType => _unitModelTypeRepository.Value;

        public IUnitReadingRepository UnitReading => _unitReadingRepository.Value;

        public IUnitRepository Unit => _unitRepository.Value;

        public IUnitTypeRepository UnitType => _unitTypeRepository.Value;

        public IUserRepository User => _userRepository.Value;

        public IRouteRepository Route => _routeRepository.Value;

        public IRouteUnitsRepository RouteUnit => _routeUnitRepository.Value;

        public IUserAlertRepository UserAlert => _userAlertRepository.Value;

        public IDamageRepository Damage => _damageRepository.Value;

        public IDamageHistoryRepository DamageHistory => _damageHistoryRepository.Value;

        public IUserAlertSendRepository UserAlertSend => _userAlertSendRepository.Value;

        public void Save() => _repositoryContext.SaveChanges();

        public async Task SaveAsync() => await _repositoryContext.SaveChangesAsync();
    }
}
