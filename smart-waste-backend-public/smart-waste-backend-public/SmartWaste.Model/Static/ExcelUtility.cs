using SmartWaste.Model.Constants;
using SmartWaste.Model.Core;
using SmartWaste.Model.Extensions;
using ClosedXML.Excel;

namespace SmartWaste.Model.Static;

public static class ExcelUtility
{
    public static byte[] GenerateExcel(List<User> users)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Users");
            // Write header
            worksheet.Cell(1, 1).Value = "Name";
            worksheet.Cell(1, 2).Value = "Role";
            worksheet.Cell(1, 3).Value = "Parent";
            worksheet.Cell(1, 4).Value = "Email";
            worksheet.Cell(1, 5).Value = "Country";
            worksheet.Cell(1, 6).Value = "Status";

            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];
                worksheet.Cell(i + 2, 1).Value = user.Name;
                worksheet.Cell(i + 2, 2).Value = user.Role.Name;
                worksheet.Cell(i + 2, 3).Value = user.Parent?.Name;
                worksheet.Cell(i + 2, 4).Value = user.Email;
                worksheet.Cell(i + 2, 5).Value = user.Country?.Name;
                worksheet.Cell(i + 2, 6).Value = user.Status?.Name;
            }

            using (var memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }

    public static byte[] GenerateExcel(List<UnitReading> unitReadings, List<AlertType> allAlerts, int timezoneOffset = 0)
    {
        using (var workbook = new XLWorkbook())
        {
            var unit = unitReadings.First().Unit;
            var worksheet = workbook.Worksheets.Add("Unit Readings");
            
            // Add unit data
            worksheet.Cell(1, 1).Value = unit.Location;
            worksheet.Cell(2, 1).Value = unit.SerialNumber;

            // Write header
            if (unit.UnitTypeId == DatabaseConstants.UnitType.NonCompacting.Id)
            {
                worksheet.Cell(4, 1).Value = "Bin Level";
                worksheet.Cell(4, 2).Value = "Battery";
                worksheet.Cell(4, 3).Value = "Flap opening";
                worksheet.Cell(4, 4).Value = "Temp °C";
                worksheet.Cell(4, 5).Value = "Compactions";
                worksheet.Cell(4, 6).Value = "Reading time";
                worksheet.Cell(4, 7).Value = "Alerts";
            }
            else
            {
                worksheet.Cell(4, 1).Value = "Bin Level";
                worksheet.Cell(4, 2).Value = "Bin Level BC";
                worksheet.Cell(4, 3).Value = "Battery";
                worksheet.Cell(4, 4).Value = "Flap opening";
                worksheet.Cell(4, 5).Value = "Temp °C";
                worksheet.Cell(4, 6).Value = "Compactions";
                worksheet.Cell(4, 7).Value = "Reading time";
                worksheet.Cell(4, 8).Value = "Alerts";
            }

            for (int i = 0; i < unitReadings.Count; i++)
            {
                var unitReading = unitReadings[i];
                int row = i + 5;
                var alertNames = unitReading.GetAlertTypes(allAlerts, includeDeviceNotReporting: false).Select(at => at.Name);

                if (unit.UnitTypeId == DatabaseConstants.UnitType.NonCompacting.Id)
                {
                    worksheet.Cell(row, 1).Value = $"{unit.ComputeUnitFillLevelPercentages(unitReading).FillLevelPercentage} %";
                    worksheet.Cell(row, 2).Value = $"{unitReading.BatteryVoltage.ConvertBatteryVoltage()} V";
                    worksheet.Cell(row, 3).Value = unitReading.NoFlapOpenings;
                    worksheet.Cell(row, 4).Value = unitReading.Temperature;
                    worksheet.Cell(row, 5).Value = unitReading.NoCompactions;
                    worksheet.Cell(row, 6).Value = unitReading.ReadingDate != null ? unitReading.ReadingDate.Value.AddHours(timezoneOffset) : "-";
                    worksheet.Cell(row, 7).Value = string.Join(", ", alertNames);
                }
                else
                {
                    worksheet.Cell(row, 1).Value = $"{unit.ComputeUnitFillLevelPercentages(unitReading).FillLevelPercentage} %";
                    worksheet.Cell(row, 2).Value =  $"{unit.ComputeUnitFillLevelPercentages(unitReading).FillLevelBcPercentage} %";
                    worksheet.Cell(row, 3).Value = $"{unitReading.BatteryVoltage.ConvertBatteryVoltage()} V";
                    worksheet.Cell(row, 4).Value = unitReading.NoFlapOpenings;
                    worksheet.Cell(row, 5).Value = unitReading.Temperature;
                    worksheet.Cell(row, 6).Value = unitReading.NoCompactions;
                    worksheet.Cell(row, 7).Value = unitReading.ReadingDate != null ? unitReading.ReadingDate.Value.AddHours(timezoneOffset) : "-";
                    worksheet.Cell(row, 8).Value = string.Join(", ", alertNames);
                }
            }

            using (var memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }

    public static byte[] GenerateExcel(List<Unit> units)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Units");
            // Write header
            worksheet.Cell(1, 1).Value = "Content type";
            worksheet.Cell(1, 2).Value = "Location";
            worksheet.Cell(1, 3).Value = "Model";
            worksheet.Cell(1, 4).Value = "Filled";
            worksheet.Cell(1, 5).Value = "Battery";
            worksheet.Cell(1, 6).Value = "Temp";
            worksheet.Cell(1, 7).Value = "Serial No";

            for (int i = 0; i < units.Count; i++)
            {
                var unit = units[i];
                worksheet.Cell(i + 2, 1).Value = unit?.ContentType?.Name;
                worksheet.Cell(i + 2, 2).Value = unit?.Location;
                worksheet.Cell(i + 2, 3).Value = unit?.UnitModelType?.Name;
                worksheet.Cell(i + 2, 4).Value = unit?.ComputeUnitFillLevelPercentages(unit?.LatestUnitReadings?.FirstOrDefault())?.FillLevelPercentage;
                worksheet.Cell(i + 2, 5).Value = $"{unit?.LatestUnitReadings?.FirstOrDefault()?.BatteryVoltage.ConvertBatteryVoltage()} V";
                worksheet.Cell(i + 2, 6).Value = unit?.LatestUnitReadings?.FirstOrDefault()?.Temperature;
                worksheet.Cell(i + 2, 7).Value = unit?.SerialNumber;
            }

            using (var memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}