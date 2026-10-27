using SmartWaste.Model.DataTransferObjects.Account;
using SmartWaste.Model.DataTransferObjects.WebConfiguration;

namespace SmartWaste.Model.DataTransferObjects.Damage
{
    public class DamageDetailsDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int? StatusId { get; set; }
        public DamageTypeDto DamageType { get; set; }
        public DateTime CreateDate { get; set; }
        public List<DamageHistoryDto> DamageHistories { get; set; } = new();
    }
}
