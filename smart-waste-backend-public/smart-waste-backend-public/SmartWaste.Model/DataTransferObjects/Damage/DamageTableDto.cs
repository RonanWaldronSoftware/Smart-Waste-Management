using SmartWaste.Model.DataTransferObjects.WebConfiguration;

namespace SmartWaste.Model.DataTransferObjects.Damage
{
    public class DamageTableDto
    {
        public int Id { get; set; }
        public int? StatusId { get; set; }
        public DamageTypeDto DamageType { get; set; }
        public UnitDamageTableDto Unit {  get; set; }
        public string? Image {  get; set; }
        public string? UserName {  get; set; }
        public string? AssignedUserName {  get; set; }

    }

    public class UnitDamageTableDto
    {
        public int Id { get; set; }
        public string? Location { get; set; }
        public string ModelTypeName {  get; set; }
    }
}
