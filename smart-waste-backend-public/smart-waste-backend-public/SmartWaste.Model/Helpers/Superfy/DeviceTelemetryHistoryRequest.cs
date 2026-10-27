namespace SmartWaste.Model.Helpers.Superfy;

public class DeviceTelemetryHistoryRequest
{
    public string DeviceId { get; set; } // UUID
    public int PageSize { get; set; } = 100; 
    public string? NextToken { get; set; }
}
