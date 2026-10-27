namespace SmartWaste.Model.Helpers.Superfy;

public class SuperfyCompanyDeviceListRequest
{
    public string? SearchKeyword { get; set; }
    public List<string> DeviceTypeIds { get; set; } = new();
    public List<string> DeviceIds { get; set; } = new();
    public bool? AssetAllocationStatus { get; set; }
    public string? ConnectivityStatus { get; set; }
    public string? LifeCycleStatus { get; set; } 
    public bool? IsContainerCompatible { get; set; }
    public string? WarrantyStatus { get; set; }
    public List<string> SortBy { get; set; } = new() { "id" };
    public List<string> SortOrder { get; set; } = new() { "asc" };
    public int Page { get; set; } = 0;
    public int PageSize { get; set; } = 50;
}
