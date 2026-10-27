namespace SmartWaste.Model.DataTransferObjects
{
    public class PaginatedResponse<T>
    {
        public int Count { get; set; }
        public List<T>? Items { get; set; }
    }
}
