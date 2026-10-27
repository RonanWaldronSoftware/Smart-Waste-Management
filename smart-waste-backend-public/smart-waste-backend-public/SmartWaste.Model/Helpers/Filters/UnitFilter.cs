namespace SmartWaste.Model.Helpers.Filters
{
    /// <summary>
    /// Filter for map
    /// </summary>
    public class UnitFilter
    {
        public List<int> ContentTypeIds { get; set; } = new();
        public List<int> FullnessStatusIds { get; set; } = new();
        public List<int> UnitTypesIds { get; set; } = new();
        public List<int> UnitModelTypeIds { get; set; } = new();
        public List<int> UserIds { get; set; } = new(); // Unit owners
    }
}
