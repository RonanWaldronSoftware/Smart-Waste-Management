namespace SmartWaste.Model.Configuration
{
    public class Configuration
    {
        public const string Section = "Configuration";
        public string BaseURLFront { get; set; }
        public string WebPage { get; set; }
        public string ContactEmail { get; set; }
        public TimeSpan StartWorkingTime { get; set; }
        public TimeSpan EndWorkingTime { get; set; }
        public int NumberOfDaysForOneTimeTokenDelete { get; set; }
        public TcpConfiguration TcpConfiguration { get; set; } = null!;
        public RabbitMQConfiguration RabbitMQConfiguration { get; set; } = null!;
        public SmsConfiguration SmsConfiguration { get; set; }
        public EmailConfiguration EmailConfiguration { get; set; }
        public FcmNotificationConfiguration FcmNotificationConfiguration { get; set; }
        public AWSS3Configuration AWSS3Configuration { get; set; } = null!;
        public SuperfyConfiguration SuperfyConfiguration { get; set; } = null!;

    }
}
