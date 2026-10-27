namespace SmartWaste.Model.DataTransferObjects.WebConfiguration
{
    public class AlertTypeDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public int? AlertTypeCategoryId { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
