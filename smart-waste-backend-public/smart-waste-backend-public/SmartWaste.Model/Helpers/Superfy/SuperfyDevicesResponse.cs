using System.Text.Json.Serialization;

namespace SmartWaste.Model.Helpers.Superfy;

public class SuperfyDevicesResponse
{
    [JsonPropertyName("items")]
    public List<Item> Items { get; set; }

    [JsonPropertyName("pagination")]
    public PaginationSuperfy PaginationSuperfy { get; set; }
}

   public class Asset
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class Company
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class Container
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("serial_number")]
        public string SerialNumber { get; set; }
    }

    public class DeviceType
    {
        [JsonPropertyName("is_superfy")]
        public bool IsSuperfy { get; set; }

        [JsonPropertyName("is_third_party")]
        public bool IsThirdParty { get; set; }

        [JsonPropertyName("is_generic")]
        public bool IsGeneric { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("company_id")]
        public int? CompanyId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("manufacturer")]
        public string? Manufacturer { get; set; }

        [JsonPropertyName("original_name")]
        public string OriginalName { get; set; }

        [JsonPropertyName("presigned_image_url")]
        public string? PresignedImageUrl { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

    public class Item
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("device_id")]
        public string DeviceId { get; set; }

        [JsonPropertyName("company")]
        public Company Company { get; set; }

        [JsonPropertyName("customer")]
        public string Customer { get; set; }

        [JsonPropertyName("serial")]
        public string Serial { get; set; }

        [JsonPropertyName("device_type")]
        public DeviceType DeviceType { get; set; }

        [JsonPropertyName("asset")]
        public Asset Asset { get; set; }

        [JsonPropertyName("container")]
        public Container Container { get; set; }

        [JsonPropertyName("connectivity_status")]
        public string? ConnectivityStatus { get; set; }

        [JsonPropertyName("first_heartbeat_ts")]
        public DateTime? FirstHeartbeatTs { get; set; }

        [JsonPropertyName("latest_heartbeat_ts")]
        public DateTime? LatestHeartbeatTs { get; set; }

        [JsonPropertyName("life_cycle_status")]
        public string LifeCycleStatus { get; set; }

        [JsonPropertyName("is_demo")]
        public bool IsDemo { get; set; }

        [JsonPropertyName("customer_shipping_date")]
        public DateTime? CustomerShippingDate { get; set; }

        [JsonPropertyName("warranty_status")]
        public string? WarrantyStatus { get; set; }

        [JsonPropertyName("battery_life_days")]
        public int? BatteryLifeDays { get; set; }

        [JsonPropertyName("firmware_version")]
        public string FirmwareVersion { get; set; }

        [JsonPropertyName("configured_heartbeat_s")]
        public int? ConfiguredHeartbeatS { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

    public class PaginationSuperfy
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }
    }

