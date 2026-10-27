namespace SmartWaste.Model.DataTransferObjects.Unit
{
    public class UnitReadingPagintetedResponse<T> : PaginatedResponse<T>
    {
        public string? Location { get; set; }
        public string? SerialNumber { get; set; }
    }

}
