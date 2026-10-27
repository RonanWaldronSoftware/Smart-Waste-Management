using SmartWaste.Model.Configuration;
using SmartWaste.Model.Constants;
using SmartWaste.Model.Core;
using SmartWaste.Model.Enums;
using SmartWaste.Model.Helpers.FcmNotification;
using SmartWaste.Model.Helpers.RabbitMQ;
using SmartWaste.Model.Resources;
using SmartWaste.Repository;
using SmartWaste.Service.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text;

namespace SmartWaste.Service.Implementations
{
    public class AlertService : IAlertService
    {
        private readonly IDbContextFactory<RepositoryContext> _contextFactory;
        private readonly IRabbitMQService _rabbitMQService;
        private readonly Configuration _configuration;
        private readonly LocalizerService _localizer;

        private List<int> processedUsers = new List<int>();

        public AlertService(IOptions<Configuration> configuration, IDbContextFactory<RepositoryContext> contextFactory, IRabbitMQService rabbitMQService, LocalizerService localizer)
        {
            _configuration = configuration.Value;
            _rabbitMQService = rabbitMQService;
            _contextFactory = contextFactory;
            _localizer = localizer;
        }
        IRepositoryManager RepositoryManager { get => new RepositoryManager(_contextFactory.CreateDbContext()); }

        public async Task SendAlert(int userId, List<AlertType> alertTypes, Unit unit, bool enableRecursion = true, bool resetList = true)
        {
            var repositoryManager = RepositoryManager;

            // On first iteration clear processed users
            if (resetList)
                processedUsers.Clear();

            // Get user and user alerts from db
            var user = await repositoryManager.User.GetUserWithAlerts(userId, false);
            var userLanguage = user.Language.Code;
            var userAlerts = user.UserAlertUsers;

            // Check if user was already processed
            if (processedUsers.Contains(userId))
                return;

            // Add user to processed users
            processedUsers.Add(userId);

            // Check if user is active
            if (user.StatusId != DatabaseConstants.Status.Inactive.Id)
            {
                // Set current user time zone time
                DateTime currentUserDateTime = DateTime.UtcNow.AddMinutes((double)(user.Country?.UtcOffsetInMin ?? 0));
                TimeSpan currentUserTime = currentUserDateTime.TimeOfDay;

                // If it's not between start and end working time, don't send notification
                if (currentUserTime < _configuration.StartWorkingTime || currentUserTime > _configuration.EndWorkingTime)
                    return;

                // Delete user alerts send which are not any of alert types
                await repositoryManager.UserAlertSend.DeleteUserAlertsSendExcludingTypes(alertTypes, userId, unit.Id);

                foreach (var alertType in alertTypes)
                {
                    // Battery Over Temperature is restricted to SmartWaste admins only:
                    // non-admin recipients never receive it, regardless of their subscription.
                    if (alertType.Id == (int)AlertTypeEnum.BatteryOverTemperature && user.RoleId != DatabaseConstants.Role.Admin.Id)
                        continue;

                    // Find relevant alert for the user
                    var userAlert = userAlerts.FirstOrDefault(ua => ua.AlertTypeId == alertType.Id && ua.CustomerUserId == unit.UserId);

                    // Send notifications only when it's not send already
                    if (userAlert != null && !userAlert.UserAlertSends.Any())
                    {
                        // Create that user alert send
                        var userAlertSend = new UserAlertSend
                        {
                            UnitId = unit.Id,
                            UserAlertId = userAlert.Id,
                        };

                        await repositoryManager.UserAlertSend.CreateUserSendAlert(userAlertSend);

                        // Init notification queue model
                        var notficationQueueModel = new QueueNotificationActionModel();

                        // Check if SMS notification should be sent
                        if (userAlerts.Any(ua => ua.AlertTypeId == (int)AlertTypeEnum.Sms && !string.IsNullOrEmpty(userAlert.User?.PhoneNumber)))
                        {
                            notficationQueueModel.Sms = new SmsMessage
                            {
                                Message = RemoveDiacritics(String.Format(_localizer.GetLocalString("alertSmsBody", userLanguage),
                                    _localizer.GetLocalString(alertType.TranslationKey, userLanguage), unit.SerialNumber, unit.Location, _configuration.WebPage)),
                                PhoneNumber = userAlert.User.PhoneNumber
                            };
                        }

                        // Check if Email notification should be sent
                        if (userAlerts.Any(ua => ua.AlertTypeId == (int)AlertTypeEnum.Email))
                        {
                            notficationQueueModel.Email = new EmailRabbitMqMessage
                            {
                                Message = String.Format(_localizer.GetLocalString("userAlertEmailBody", userLanguage),
                                    _localizer.GetLocalString(alertType.TranslationKey, userLanguage), unit.Location, unit.SerialNumber,
                                    _configuration.WebPage, _configuration.ContactEmail),
                                SendTo = userAlert.User.Email,
                                Subject = String.Format(_localizer.GetLocalString("userAlertEmailTitle", userLanguage),
                                    _localizer.GetLocalString(alertType.TranslationKey, userLanguage))
                            };
                        }

                        // Check if Push notification should be sent
                        if (userAlerts.Any(ua => ua.AlertTypeId == (int)AlertTypeEnum.Push))
                        {
                            foreach (var deviceToken in user.DeviceTokens)
                            {
                                notficationQueueModel.FcmPushNotification = new FcmPushNotificationModel
                                {
                                    Title = String.Format(_localizer.GetLocalString("userAlertTitle", userLanguage), _localizer.GetLocalString(alertType.TranslationKey, userLanguage)),
                                    Body = String.Format(_localizer.GetLocalString("alertPushNotificationBody", userLanguage), unit.SerialNumber, unit.Location, _configuration.WebPage),
                                    DeviceId = deviceToken.DeviceToken1 ?? ""
                                };
                            }
                        }

                        // Check if at least one notification is ready to be sent
                        if (notficationQueueModel.Sms != null || notficationQueueModel.Email != null || notficationQueueModel.FcmPushNotification != null)
                        {
                            _rabbitMQService.SendMessage(notficationQueueModel, GlobalConstants.NotificationQueue);
                        }
                    }
                }
            }

            if (enableRecursion)
            {
                // If user has a parent send notifications for parent
                if (user.ParentId != null)
                    await SendAlert(user.ParentId.Value, alertTypes, unit, true, false);

                // Send alerts for child technician and driver users
                var childUsers = user.InverseParent.ToList();
                foreach (var childUser in childUsers.Where(childUser => childUser.RoleId == DatabaseConstants.Role.Technician.Id || childUser.RoleId == DatabaseConstants.Role.Driver.Id))
                    await SendAlert(childUser.Id, alertTypes, unit, false, false);
            }
        }

        /// <summary>
        /// Method for removing diacritics from sms
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        static string RemoveDiacritics(string? text)
        {
            if (text is null)
                return "";

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char c = normalizedString[i];
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder
                .ToString()
                .Normalize(NormalizationForm.FormC);
        }
    }
}
