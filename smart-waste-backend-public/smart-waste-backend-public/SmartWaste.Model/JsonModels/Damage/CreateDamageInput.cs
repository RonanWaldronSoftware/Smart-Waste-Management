using SmartWaste.Model.Helpers;

namespace SmartWaste.Model.JsonModels.Damage
{
    public class CreateDamageInput
    {
        public int? DamageTypeId { get; set; }
        public string SerialNumber { get; set; }
        public FileData Image {  get; set; }
        public string? Comment { get; set; }
    }
}
