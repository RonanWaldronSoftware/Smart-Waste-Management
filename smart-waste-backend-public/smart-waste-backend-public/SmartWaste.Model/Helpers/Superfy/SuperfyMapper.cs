using SmartWaste.Model.Core;
using SmartWaste.Model.Extensions;

namespace SmartWaste.Model.Helpers.Superfy;

public static class SuperfyMapper
{
    public static List<UnitReading> MapTelemetryHistoryToUnitReadings(SuperfyTelemetryHistoryResponse response, Unit unit)
    {
        if (response?.Telemetry == null)
            return new List<UnitReading>();

        return response.Telemetry.Select(telemetry => 
        {
            var unitReading = new UnitReading
            {
                SerialNumber = unit.SerialNumber ?? "",
                UnitId = unit.Id,
                Source = "Api",
                Temperature = (int?)Math.Round(telemetry.Temperature),
                BinLevel = (int?)Math.Round(telemetry.LaserAverageDistance),
                BinLevelBc = (int?)Math.Round(telemetry.LaserAverageDistance),
                CreateDate = telemetry.Timestamp.ToJsonFormat(),
                ReadingDate = telemetry.Timestamp.ToJsonFormat(),
                Unit = unit // Set the Unit navigation property for AutoMapper
            };

            // Calculate fill levels and fullness status
            // var fillLevel = unit.ComputeUnitFillLevelPercentages(unitReading);
            // unitReading.BinLevel = fillLevel.FillLevelPercentage;
            // unitReading.BinLevelBc = fillLevel.FillLevelBcPercentage;
            // unitReading.FullnessStatusId = fillLevel.FillnessStatusId;

            return unitReading;
        }).ToList();
    }

    public static UnitReading MapTelemetryToUnitReading(Telemetry telemetry, Unit unit)
    {
        if (telemetry == null)
            return null;

        var unitReading = new UnitReading
        {
            SerialNumber = unit.SerialNumber ?? "",
            UnitId = unit.Id,
            Source = "Api",
            Temperature = (int?)Math.Round(telemetry.Temperature),
            BinLevel = (int?)Math.Round(telemetry.LaserAverageDistance),
            BinLevelBc = (int?)Math.Round(telemetry.LaserAverageDistance),
            ReadingDate = telemetry.Timestamp.ToJsonFormat(),
            CreateDate = telemetry.Timestamp.ToJsonFormat(),
            Unit = unit // Set the Unit navigation property for AutoMapper
        };

        return unitReading;
    }

    public static LatestUnitReading MapTelemetryToLatestUnitReading(Telemetry telemetry, Unit unit)
    {
        if (telemetry == null)
            return null;

        var latestReading = new LatestUnitReading
        {
            SerialNumber = unit.SerialNumber ?? "",
            UnitId = unit.Id,
            Source = "Api",
            Temperature = (int?)Math.Round(telemetry.Temperature),
            BinLevel = (int?)Math.Round(telemetry.LaserAverageDistance),
            BinLevelBc = (int?)Math.Round(telemetry.LaserAverageDistance),
            ReadingDate = telemetry.Timestamp.ToJsonFormat(),
            CreateDate = telemetry.Timestamp.ToJsonFormat(),
            Unit = unit // Set the Unit navigation property for AutoMapper
        };

        // Calculate fill levels and fullness status
        var fillLevel = unit.ComputeUnitFillLevelPercentages(latestReading);
        latestReading.BinLevelPercentage = fillLevel.FillLevelPercentage;
        latestReading.FullnessStatusId = fillLevel.FillnessStatusId;

        return latestReading;
    }
}