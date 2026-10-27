using SmartWaste.Model.DataTransferObjects.Account;

namespace SmartWaste.Model.DataTransferObjects.Damage
{
    public class DamageHistoryDto
    {
        public int? StatusId { get; set; }
        public string? Comment { get; set; }
        public DateTime CreateDate { get; set; }
        public string? Image {  get; set; }
        public ShortUserDto AssignedUser {  get; set; }
    }
}
