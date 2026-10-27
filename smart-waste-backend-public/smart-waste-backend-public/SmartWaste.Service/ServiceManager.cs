using AutoMapper;
using SmartWaste.Model.Configuration;
using SmartWaste.Model.Core;
using SmartWaste.Model.Resources;
using SmartWaste.Repository;
using SmartWaste.Service.Contracts;
using SmartWaste.Service.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SmartWaste.Service
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IAccountService> _accountService;
        private readonly Lazy<IConfigurationService> _configurationService;
        private readonly Lazy<IRabbitMQService> _rabbitmqService;
        private readonly Lazy<IEmailService> _emailService;
        private readonly Lazy<ISmsService> _smsService;
        private readonly Lazy<IFcmNotificationService> _fcmNotificationService;
        private readonly Lazy<IUserService> _userService;
        private readonly Lazy<ISendService> _sendService;
        private readonly Lazy<IFileService> _fileService;
        private readonly Lazy<IAlertService> _alertService;
        private readonly Lazy<IUnitReadingService> _unitReadingService;
        private readonly Lazy<ISuperfyService> _superfyService;

        public ServiceManager(IOptions<Configuration> configuration, IHttpClientFactory httpClientFactory,
                              IRepositoryManager repository, IMapper mapper, LocalizerService localizer,
                              ILoggerFactory loggerFactory, IDbContextFactory<RepositoryContext> contextFactory)
        {
            _accountService = new Lazy<IAccountService>(() => new AccountService(configuration, repository));
            _configurationService = new Lazy<IConfigurationService>(() => new ConfigurationService(repository, mapper, localizer));
            _rabbitmqService = new Lazy<IRabbitMQService>(() => new RabbitMQService(configuration));
            _emailService = new Lazy<IEmailService>(() => new EmailService(loggerFactory.CreateLogger<EmailService>(), configuration));
            _smsService = new Lazy<ISmsService>(() => new SmsService(httpClientFactory.CreateClient(), configuration, loggerFactory.CreateLogger<SmsService>()));
            _fcmNotificationService = new Lazy<IFcmNotificationService>(() => new FcmNotificationService(configuration));
            _userService = new Lazy<IUserService>(() => new UserService(repository, mapper, localizer));
            _sendService = new Lazy<ISendService>(() => new SendService(_rabbitmqService.Value));
            _fileService = new Lazy<IFileService>(() => new AWS3FileService(loggerFactory.CreateLogger<AWS3FileService>(), configuration));
            _alertService = new Lazy<IAlertService>(() => new AlertService(configuration, contextFactory, _rabbitmqService.Value, localizer));
            _unitReadingService = new Lazy<IUnitReadingService>(() => new UnitReadingService(contextFactory, mapper));
            _superfyService = new Lazy<ISuperfyService>(() => new SuperfyService(httpClientFactory.CreateClient(), configuration,loggerFactory.CreateLogger<SuperfyService>()));
        }

        public IAccountService AccountService => _accountService.Value;

        public IConfigurationService ConfigurationService => _configurationService.Value;

        public IRabbitMQService RabbitMQService => _rabbitmqService.Value;

        public ISendService SendService => _sendService.Value;

        public ISmsService SmsService => _smsService.Value;

        public IUserService UserService => _userService.Value;

        public IFcmNotificationService FcmNotificationService => _fcmNotificationService.Value;

        public IEmailService EmailService => _emailService.Value;

        public IFileService FileService => _fileService.Value;

        public IAlertService AlertService => _alertService.Value;
        
        public IUnitReadingService UnitReadingService => _unitReadingService.Value;
        public ISuperfyService SuperfyService => _superfyService.Value;
    }
}
