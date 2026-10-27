using SmartWaste.Model.Core;
using SmartWaste.Model.Extensions;
using System.Text;

namespace SmartWaste.Model.Static
{
    public static class CsvUtility
    {
        public static byte[] GenerateCsv(List<User> users)
        {
            string header = "Name;Role;Parent;Email;Country;Status";
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new StreamWriter(memoryStream, Encoding.UTF8))
                {
                    // Write header
                    writer.WriteLine(header);

                    foreach (var user in users)
                    {
                        // Add details to line 
                        string line = $"{user.Name};{user.Role.Name};{user.Parent?.Name};{user.Email};{user.Country?.Name};{user.Status?.Name}";

                        // Write line
                        writer.WriteLine(line);
                    }
                }
                return memoryStream.ToArray();
            }
        }
        public static byte[] GenerateCsv(List<UnitReading> unitReadings, List<AlertType> allAlerts)
        {
            string header = "Bin Level;Bin Level BC;Battery;Flap opening;Temp °C;Compactions;Reading time;Alerts";
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new StreamWriter(memoryStream, Encoding.UTF8))
                {
                    // Write header
                    writer.WriteLine(header);
                    foreach (var unitReading in unitReadings)
                    {
                        // Get alert names
                        var alertNames = unitReading.GetAlertTypes(allAlerts, includeDeviceNotReporting: false).Select(at => at.Name);

                        // Add details to line 
                        string line = $"{unitReading.BinLevel};{unitReading.BinLevelBc};{unitReading.BatteryVoltage.ConvertBatteryVoltage()} V;{unitReading.NoFlapOpenings};" +
                            $"{unitReading.Temperature};{unitReading.NoCompactions};{unitReading.ReadingDate};{unitReading.ReadingDate};{string.Join("\n", alertNames)}";

                        // Write line
                        writer.WriteLine(line);
                    }
                }
                return memoryStream.ToArray();
            }
        }
        public static byte[] GenerateCsv(List<Unit> units)
        {
            string header = "Content type;Location;Model;Filled;Battery;Temp;Serial No";
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new StreamWriter(memoryStream, Encoding.UTF8))
                {
                    // Write header
                    writer.WriteLine(header);
                    foreach (var unit in units)
                    {
                        // Add details to line 
                        string line = $"{unit?.ContentType?.Name};{unit?.Location};{unit?.UnitModelType?.Name};" +
                        $"{unit?.ComputeUnitFillLevelPercentages(unit?.LatestUnitReadings?.FirstOrDefault())?.FillLevelPercentage};{unit?.LatestUnitReadings?.FirstOrDefault()?.BatteryVoltage.ConvertBatteryVoltage()} V;" +
                            $"{unit?.LatestUnitReadings?.FirstOrDefault()?.Temperature};{unit?.SerialNumber}";

                        // Write line
                        writer.WriteLine(line);
                    }
                }
                return memoryStream.ToArray();
            }
        }
    }
}

