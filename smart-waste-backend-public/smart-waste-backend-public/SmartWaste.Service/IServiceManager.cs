using SmartWaste.Service.Contracts;

namespace SmartWaste.Service
{
    public interface IServiceManager
    {
        public IAccountService AccountService { get; }
        public IConfigurationService ConfigurationService { get; }
        public IRabbitMQService RabbitMQService { get; }
        public IEmailService EmailService { get; }
        public ISmsService SmsService { get; }
        public IFcmNotificationService FcmNotificationService { get; }
        public IUserService UserService { get; }
        public ISendService SendService { get; }
        public IFileService FileService { get; }
        public IAlertService AlertService { get; }
        public IUnitReadingService UnitReadingService { get; }
        public ISuperfyService SuperfyService { get; }
    }
}
