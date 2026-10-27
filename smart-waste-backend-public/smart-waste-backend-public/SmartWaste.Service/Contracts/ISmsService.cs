namespace SmartWaste.Service.Contracts
{
    public interface ISmsService
    {
        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task SendSMS(string phoneNumber, string message);
    }
}
