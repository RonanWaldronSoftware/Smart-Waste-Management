namespace SmartWaste.Model.DataTransferObjects.WebConfiguration
{
    public class UnitModelTypeDto : ShortUnitModelTypeDto
    {
        public int EmptyLevel { get; set; }

        public int FullLevel { get; set; }

        public int Capacity { get; set; }
    }

    public class ShortUnitModelTypeDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }
    }
}
