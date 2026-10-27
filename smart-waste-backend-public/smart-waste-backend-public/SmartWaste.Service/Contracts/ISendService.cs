namespace SmartWaste.Service.Contracts;

public interface ISendService
{
    /// <summary>
    /// Send sms data
    /// </summary>
    /// <param name="message"></param>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    void SendSms(string message, string phoneNumber);

    /// <summary>
    /// Send email to specific email
    /// </summary>
    /// <param name="message"></param>
    /// <param name="subject"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    void SendEmail(string message, string subject, string to);

    /// <summary>
    /// Send notification to a specific device
    /// </summary>
    /// <param name="message"></param>
    /// <param name="title"></param>
    /// <param name="deviceToken"></param>
    void SendPushNotification(string message, string title, string deviceToken);
}