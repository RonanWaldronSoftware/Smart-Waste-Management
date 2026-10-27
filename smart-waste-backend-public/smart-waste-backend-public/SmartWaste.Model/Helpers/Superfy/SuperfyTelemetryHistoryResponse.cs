using System.Text.Json.Serialization;

namespace SmartWaste.Model.Helpers.Superfy;

public class SuperfyTelemetryHistoryResponse
{
    [JsonPropertyName("telemetry")]
    public List<Telemetry> Telemetry { get; set; }

    [JsonPropertyName("pagination")]
    public Pagination Pagination { get; set; }
}

public class Telemetry
{
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    [JsonPropertyName("laser_distance_1")]
    public double LaserDistance1 { get; set; }

    [JsonPropertyName("laser_distance_2")]
    public double LaserDistance2 { get; set; }

    [JsonPropertyName("laser_average_distance")]
    public double LaserAverageDistance { get; set; }

    [JsonPropertyName("ts")]
    public DateTime Timestamp { get; set; }
}

public class Pagination
{
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    [JsonPropertyName("next_token")]
    public string NextToken { get; set; }
}
