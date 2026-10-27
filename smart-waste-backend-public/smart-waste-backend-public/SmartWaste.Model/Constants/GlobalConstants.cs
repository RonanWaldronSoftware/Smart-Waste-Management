namespace SmartWaste.Model.Constants
{
    public class GlobalConstants
    {
        /// <summary>
        /// RabbitMQ notification queue
        /// </summary>
        public const string NotificationQueue = "smartWaste-notification";
        /// <summary>
        /// RabbitMQ unit queue
        /// </summary>
        public const string UnitQueue = "smartWaste-unitData";
        /// <summary>
        /// Main damage folder name for s3 storage
        /// </summary>
        public const string S3DamageFolder = "damage";
        /// <summary>
        /// Main installs folder name for s3 storage
        /// </summary>
        public const string S3InstallsFolder = "installs";
    }
}
