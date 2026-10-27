using SmartWaste.Model.JsonModels.Superfy;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using SmartWaste.Model.Configuration;
using SmartWaste.Model.Helpers.Superfy;
using SmartWaste.Service.Contracts;
using Microsoft.Extensions.Options;

namespace SmartWaste.Service.Implementations;

public class SuperfyService : ISuperfyService
{
    private readonly HttpClient _httpClient;
    private readonly SuperfyConfiguration _superfyConfiguration;
    private readonly ILogger<SuperfyService> _logger;

    private string _cachedToken;

    public SuperfyService(HttpClient httpClient, IOptions<Configuration> configuration, ILogger<SuperfyService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _superfyConfiguration = configuration.Value.SuperfyConfiguration;
    }

    public async Task<Telemetry?> GetDeviceTelemetryLatest(string deviceId)
    {
        try
        {
            var token = await GetTokenAsync();

            var request = new HttpRequestMessage
            (
                HttpMethod.Get,
                $"{_superfyConfiguration.BaseUrl}devices/public/api/devices/{deviceId}/telemetry/latest"
            );

            request.Headers.Add("authorization", token);

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Error while getting superfy telemetry latest {responseContent}");

                return null;
            }

            return JsonSerializer.Deserialize<Telemetry>(responseContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception while getting superfy telemetry latest for device {deviceId}");
            return null;
        }
    }

    public async Task<SuperfyTelemetryHistoryResponse> GetDeviceTelemetryHistory(DeviceTelemetryHistoryRequest request)
    {
        try
        {
            var token = await GetTokenAsync();

            var url = $"{_superfyConfiguration.BaseUrl}devices/public/api/devices/{request.DeviceId}/telemetry/history";
            var queryParams = new List<string>();

            // page_size
            queryParams.Add($"page_size={request.PageSize}");

            // next_token
            if (!string.IsNullOrWhiteSpace(request.NextToken))
                queryParams.Add($"next_token={Uri.EscapeDataString(request.NextToken)}");

            if (queryParams.Any())
                url += "?" + string.Join("&", queryParams);

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
            httpRequest.Headers.Add("authorization", token);

            var response = await _httpClient.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Error while getting superfy telemetry history {responseContent}");

                return new SuperfyTelemetryHistoryResponse();
            }

            return JsonSerializer.Deserialize<SuperfyTelemetryHistoryResponse>(responseContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception while getting superfy telemetry history for device {request.DeviceId}");
            return new SuperfyTelemetryHistoryResponse();
        }
    }

    public async Task<SuperfyDevicesResponse> GetCompanyDeviceListAsync(SuperfyCompanyDeviceListRequest request)
    {
        try
        {
            var token = await GetTokenAsync();

            var url = $"{_superfyConfiguration.BaseUrl}devices/public/api/companies/{_superfyConfiguration.CompanyId}/devices";
            var queryParams = new List<string>();

            if (!string.IsNullOrWhiteSpace(request.SearchKeyword))
                queryParams.Add($"search_keyword={Uri.EscapeDataString(request.SearchKeyword)}");

            if (request.DeviceTypeIds?.Any() == true)
                queryParams.Add($"device_type_ids={string.Join(",", request.DeviceTypeIds)}");

            if (request.DeviceIds?.Any() == true)
                queryParams.Add($"device_ids={string.Join(",", request.DeviceIds)}");

            if (request.AssetAllocationStatus.HasValue)
                queryParams.Add($"asset_allocation_status={request.AssetAllocationStatus.Value.ToString().ToLower()}");

            if (!string.IsNullOrWhiteSpace(request.ConnectivityStatus))
                queryParams.Add($"connectivity_status={Uri.EscapeDataString(request.ConnectivityStatus)}");

            if (!string.IsNullOrWhiteSpace(request.LifeCycleStatus))
                queryParams.Add($"life_cycle_status={Uri.EscapeDataString(request.LifeCycleStatus)}");

            if (request.IsContainerCompatible.HasValue)
                queryParams.Add($"is_container_compatible={request.IsContainerCompatible.Value.ToString().ToLower()}");

            if (!string.IsNullOrWhiteSpace(request.WarrantyStatus))
                queryParams.Add($"warranty_status={Uri.EscapeDataString(request.WarrantyStatus)}");

            if (request.SortBy?.Any() == true)
                queryParams.Add($"sort_by={string.Join(",", request.SortBy)}");

            if (request.SortOrder?.Any() == true)
                queryParams.Add($"sort_order={string.Join(",", request.SortOrder)}");

            queryParams.Add($"page={request.Page}");
            queryParams.Add($"page_size={request.PageSize}");

            if (queryParams.Any())
                url += "?" + string.Join("&", queryParams);

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
            httpRequest.Headers.Add("authorization", token);

            var response = await _httpClient.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Error while getting superfy devices {responseContent}");
                return new SuperfyDevicesResponse();
            }

            return JsonSerializer.Deserialize<SuperfyDevicesResponse>(responseContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while getting superfy company device list");
            return new SuperfyDevicesResponse();
        }
    }

    private async Task<string> GetTokenAsync()
    {
        if (!string.IsNullOrEmpty(_cachedToken))
            return _cachedToken;

        var payload = new
        {
            username = _superfyConfiguration.Username,
            password = _superfyConfiguration.Password,
            notificationCenterType = _superfyConfiguration.NotificationCenterType,
            deviceID = _superfyConfiguration.DeviceID
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(_superfyConfiguration.AuthUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new ApplicationException($"Superfy API error: {response.StatusCode} - {error}");
        }

        var responseString = await response.Content.ReadAsStringAsync();
        var auth = JsonSerializer.Deserialize<AuthResponse>(responseString);
        
        _cachedToken = auth?.Message?.Token;
        return _cachedToken;
    }
}