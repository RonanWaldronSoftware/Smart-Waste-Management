using SmartWaste.Model.Constants;
using SmartWaste.Model.Core;
using SmartWaste.Model.Extensions;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace SmartWaste.Model.Static
{
    public static class PdfUtility
    {
        private const int _tableWidthPercentage = 100;
        private static readonly Font _headerFont = FontFactory.GetFont("Arial", 10, Font.BOLD);
        private static readonly Font _cellFont = FontFactory.GetFont("Arial", 8, Font.NORMAL);

        private static void AddCellToTable(PdfPTable table, string text, Font font)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font))
            {
                PaddingLeft = 5f,
                PaddingRight = 5f,
                PaddingBottom = 5f
            };

            table.AddCell(cell);
        }
        public static byte[] GeneratePdf(List<User> users)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Document document = new();
                PdfWriter.GetInstance(document, stream);
                document.Open();

                // Add Title
                document.Add(new Paragraph("User List"));
                document.Add(new Paragraph(" ")); // Empty line

                // Create table
                PdfPTable table = new PdfPTable(6);
                table.WidthPercentage = _tableWidthPercentage; 

                AddCellToTable(table, "Name", _headerFont);
                AddCellToTable(table, "Role", _headerFont);
                AddCellToTable(table, "Parent", _headerFont);
                AddCellToTable(table, "Email", _headerFont);
                AddCellToTable(table, "Country", _headerFont);
                AddCellToTable(table, "Status", _headerFont);

                // Add rows
                foreach (var user in users)
                {
                    AddCellToTable(table, user.Name, _cellFont);
                    AddCellToTable(table, user.Role.Name, _cellFont);
                    AddCellToTable(table, user.Parent?.Name, _cellFont);
                    AddCellToTable(table, user.Email, _cellFont);
                    AddCellToTable(table, user.Country?.Name, _cellFont);
                    AddCellToTable(table, user.Status?.Name, _cellFont);
                }

                document.Add(table);
                document.Close();

                return stream.ToArray();
            }
        }

        public static byte[] GeneratePdf(List<Unit> units)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Document document = new();
                PdfWriter.GetInstance(document, stream);
                document.Open();

                // Add Title
                document.Add(new Paragraph("Unit List"));
                document.Add(new Paragraph(" ")); // Empty line
                
                // Create table
                PdfPTable table = new PdfPTable(7);
                table.WidthPercentage = _tableWidthPercentage;

                AddCellToTable(table, "Content type", _headerFont);
                AddCellToTable(table, "Location", _headerFont);
                AddCellToTable(table, "Model", _headerFont);
                AddCellToTable(table, "Filled", _headerFont);
                AddCellToTable(table, "Battery", _headerFont);
                AddCellToTable(table, "Temp", _headerFont);
                AddCellToTable(table, "Serial No", _headerFont);
                

                // Add rows
                foreach (var unit in units)
                {
                    AddCellToTable(table, unit.ContentType?.Name, _cellFont);
                    AddCellToTable(table, unit.Location, _cellFont);
                    AddCellToTable(table, unit.UnitModelType?.Name, _cellFont);
                    AddCellToTable(table, unit.ComputeUnitFillLevelPercentages(unit?.LatestUnitReadings?.FirstOrDefault())?.FillLevelPercentage.ToString() ?? "", _cellFont);
                    AddCellToTable(table, unit.LatestUnitReadings?.FirstOrDefault()?.BatteryVoltage.HasValue == true
                        ? unit.LatestUnitReadings.First().BatteryVoltage.ConvertBatteryVoltage().ToString() + "V"
                        : "-", _cellFont);
                    AddCellToTable(table, unit.LatestUnitReadings?.FirstOrDefault()?.Temperature.ToString() ?? "0", _cellFont);
                    AddCellToTable(table, unit.SerialNumber, _cellFont);
                }

                document.Add(table);
                document.Close();

                return stream.ToArray();
            }
        }

        public static byte[] GeneratePdf(List<UnitReading> unitReadings, List<AlertType> allAlerts, int timezoneOffset = 0)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Document document = new();
                PdfWriter.GetInstance(document, stream);
                document.Open();

                // Add Title
                document.Add(new Paragraph("Unit readings List"));
                document.Add(new Paragraph(" ")); // Empty line
                
                // Get unit from readings
                var unit = unitReadings.First().Unit;
                
                // Create a table
                PdfPTable table = new PdfPTable(8);
                table.WidthPercentage = _tableWidthPercentage;

                AddCellToTable(table, "Bin Levele", _headerFont);
                
                if (unit.UnitTypeId == DatabaseConstants.UnitType.Compacting.Id)
                    AddCellToTable(table, "Bin Level BC", _headerFont);
                
                AddCellToTable(table, "Battery", _headerFont);
                AddCellToTable(table, "Flap opening", _headerFont);
                AddCellToTable(table, "Temp °C", _headerFont);
                AddCellToTable(table, "Compactions", _headerFont);
                AddCellToTable(table, "Reading time", _headerFont);
                AddCellToTable(table, "Alerts", _headerFont);

                // Add rows
                foreach (var unitReading in unitReadings)
                {
                    AddCellToTable(table, $"{unit.ComputeUnitFillLevelPercentages(unitReading).FillLevelPercentage} %", _cellFont);
                    
                    if (unit.UnitTypeId == DatabaseConstants.UnitType.Compacting.Id)
                        AddCellToTable(table,$"{unit.ComputeUnitFillLevelPercentages(unitReading).FillLevelBcPercentage} %", _cellFont);
                    
                    AddCellToTable(table, unitReading.BatteryVoltage.ConvertBatteryVoltage() + "V", _cellFont);
                    AddCellToTable(table, unitReading.NoFlapOpenings.ToString(), _cellFont);
                    AddCellToTable(table, unitReading.Temperature.ToString(), _cellFont);
                    AddCellToTable(table, unitReading.NoCompactions.ToString(), _cellFont);
                    AddCellToTable(table, unitReading.ReadingDate != null ? unitReading.ReadingDate.Value.AddHours(timezoneOffset).ToString() : "-", _cellFont);
                    
                    // Convert alert types list to string
                    var alertTypes = unitReading.GetAlertTypes(allAlerts, includeDeviceNotReporting: false);
                    var alertNames = alertTypes.Select(alert => alert.Name);
                    
                    AddCellToTable(table, string.Join("\n", alertNames), _cellFont);
                }

                document.Add(table);
                document.Close();

                return stream.ToArray();
            }
        }
    }
}
