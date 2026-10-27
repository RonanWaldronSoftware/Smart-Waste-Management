namespace SmartWaste.Model.Configuration
{
    public sealed class AWSS3Configuration
    {
        public const string Section = "AWSS3Configuration";
        public string ServiceURL { get; set; } = null!;
        public string AccessKeyId { get; set; } = null!;
        public string SecretAccessKey { get; set; } = null!;
        public string InstanceCode { get; set; } = null!;
        public string BucketName { get; set; } = null!;
    }
}
