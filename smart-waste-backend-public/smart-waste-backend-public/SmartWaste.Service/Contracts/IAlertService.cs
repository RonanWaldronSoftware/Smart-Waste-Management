using SmartWaste.Model.Core;

namespace SmartWaste.Service.Contracts
{
    public interface IAlertService
    {
        /// <summary>
        /// Method sends notifications to a user and potentially their associated users (parents and children) based on specified alert types. 
        /// It first checks if the user has already been processed to avoid duplicate alerts. The method retrieves user details and their alert preferences, 
        /// then prepares and sends SMS, email, or push notifications according to the user's preferences.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="alertTypes"></param>
        /// <param name="unit"></param>
        /// <param name="enableRecursion"></param>
        /// <param name="resetList"></param>
        /// <returns></returns>
        Task SendAlert(int userId, List<AlertType> alertTypes, Unit unit, bool enableRecursion = true, bool resetList = true);
    }
}
