namespace SmartWaste.Model.Helpers
{
    public class TableResponse<T>
    {
        public List<T> Responses { get; set; } = new List<T>();
        public DatatableOptions Options { get; set; }
    }
}
