namespace SmartWaste.Model.DataTransferObjects.Dashboard
{
    public class DashboardDto
    {
        public List<ContentTypeCountDto> ContentTypesCount { get; set; } = new();
        public int UnitsWithAlertsCount { get; set; }
        public List<FullnessLevelDto> ContentFullnessLevel { get; set; } = new();
    }

    public class FullnessLevelDto
    {
        public int ContentTypeId { get; set; }
        public int SmallFullnessCount { get; set; } = 0;
        public int MediumFullnessCount { get; set; } = 0;
        public int CriticalFullnessCount { get; set; } = 0;
        public int CriticalFullnessInLiters { get; set; } = 0;
    }

    public class ContentTypeCountDto
    {
        public int ContentTypeId { get; set; }
        public int Count { get; set; }
    }
}

