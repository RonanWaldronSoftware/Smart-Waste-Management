using SmartWaste.Model.Helpers.Email;

namespace SmartWaste.Service.Contracts
{
    public interface IEmailService
    {
        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="to"></param>
        /// <param name="message"></param>
        /// <param name="subject"></param>
        /// <param name="attachments"></param>
        /// <returns></returns>
        Task SendMail(string to, string message, string subject, IEnumerable<EmailAttachmentModel>? attachments = null);
    }
}
