namespace SmartWaste.Model.JsonModels.Damage
{
    public class UpdateDamageInput
    {
        public int DamageId { get; set; }

        public int? StatusId { get; set; }

        public string? Comment { get; set; }
        public int? AssignedUserId { get; set; }
    }
}
