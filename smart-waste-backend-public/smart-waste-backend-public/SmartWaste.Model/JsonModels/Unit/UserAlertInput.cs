namespace SmartWaste.Model.JsonModels.Unit
{
    public class UserAlertInput
    {
        public List<int> AlertTypeIds { get; set; }

        public int UserId { get; set; }

        public int CustomerUserId { get; set; }
    }
}
