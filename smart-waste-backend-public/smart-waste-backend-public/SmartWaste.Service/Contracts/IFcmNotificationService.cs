using SmartWaste.Model.Helpers.FcmNotification;

namespace SmartWaste.Service.Contracts
{
    public interface IFcmNotificationService
    {
        /// <summary>
        /// Send push notification to the user
        /// </summary>
        /// <param name="notificationModel"></param>
        /// <returns></returns>
        Task<string> SendNotification(FcmPushNotificationModel notificationModel);
    }
}
