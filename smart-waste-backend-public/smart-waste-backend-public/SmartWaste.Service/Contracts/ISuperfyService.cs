namespace SmartWaste.Service.Contracts;

using SmartWaste.Model.Helpers.Superfy;

public interface ISuperfyService
{
    Task<Telemetry?> GetDeviceTelemetryLatest(string deviceId);
    Task<SuperfyTelemetryHistoryResponse> GetDeviceTelemetryHistory(DeviceTelemetryHistoryRequest request);
    Task<SuperfyDevicesResponse> GetCompanyDeviceListAsync(SuperfyCompanyDeviceListRequest request);
}