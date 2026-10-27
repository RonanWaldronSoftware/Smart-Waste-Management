namespace SmartWaste.Model.Helpers
{
    public class DatatableOptions
    {
        public string? Search { get; set; } = "";
        public int PerPage { get; set; } = 50;
        public int Page { get; set; } = 1;
        //public int FirstShowPage { get; set; }
        //public int LastShowPage { get; set; }
        //public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public string SortBy { get; set; } = "Id";
        public string SortByDirection { get; set; } = "desc";
    }
}
