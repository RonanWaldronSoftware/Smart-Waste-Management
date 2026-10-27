using SmartWaste.Model.Core;
using SmartWaste.Model.Enums;

namespace SmartWaste.Model.Extensions
{
    public static class UnitReadingExtensions
    {

        /// <summary>
        /// Get alert types based on reading
        /// Only the alert types that match the conditions in the unit reading will be added from the provided list of all possible alert types.
        /// </summary>
        /// <param name="unitReading"></param>
        /// <param name="allPossibleAlertTypes"></param>
        /// <returns></returns>
        public static List<AlertType> GetAlertTypes(this LatestUnitReading unitReading, List<AlertType> allPossibleAlertTypes)
        {
            var alertTypesForSend = new List<AlertType>();

            // Define alert conditions
            var alertConditions = new Dictionary<Func<LatestUnitReading, bool>, AlertTypeEnum>
                {
                    { reading => reading.BatteryUvlo, AlertTypeEnum.BatteryUVLO },
                    { reading => reading.BinEmptiedLastPeriod, AlertTypeEnum.EmptiedLastPeriod },
                    { reading => reading.BatteryOverTempLo, AlertTypeEnum.BatteryOverTemperature },
                    { reading => reading.BinLocked, AlertTypeEnum.BinLocked },
                    { reading => reading.BinTilted, AlertTypeEnum.BinTilted },
                    { reading => reading.ServiceDoorOpen, AlertTypeEnum.ServiceDoorStuckOpen },
                    { reading => reading.FlapStuckOpen, AlertTypeEnum.FlapStuckOpen },
                    { reading => reading.BinFull, AlertTypeEnum.BinFull },
                };

            foreach (var condition in alertConditions)
            {
                if (condition.Key(unitReading))
                {
                    var alertForAdd = allPossibleAlertTypes.FirstOrDefault(a => a.Id == (int)condition.Value);
                    if (alertForAdd != null)
                    {
                        alertTypesForSend.Add(alertForAdd);
                    }
                }
            }

            // Check if the unit reading's create date is older than 24 hours
            if (unitReading.CreateDate < DateTime.Now.AddHours(-24))
                alertTypesForSend.Add(allPossibleAlertTypes.First(at => at.Id == (int)AlertTypeEnum.DeviceNotReporting));
            
            return alertTypesForSend;
        }

        /// <summary>
        /// Get alert types based on reading
        /// Only the alert types that match the conditions in the unit reading will be added from the provided list of all possible alert types.
        /// </summary>
        /// <param name="unitReading"></param>
        /// <param name="allPossibleAlertTypes"></param>
        /// <param name="includeDeviceNotReporting"></param>
        /// <returns></returns>
        public static List<AlertType> GetAlertTypes(this UnitReading unitReading, List<AlertType> allPossibleAlertTypes, bool includeDeviceNotReporting = true)
        {
            var alertTypesForSend = new List<AlertType>();

            // Define alert conditions
            var alertConditions = new Dictionary<Func<UnitReading, bool>, AlertTypeEnum>
                {
                    { reading => reading.BatteryUvlo, AlertTypeEnum.BatteryUVLO },
                    { reading => reading.BinEmptiedLastPeriod, AlertTypeEnum.EmptiedLastPeriod },
                    { reading => reading.BatteryOverTempLo, AlertTypeEnum.BatteryOverTemperature },
                    { reading => reading.BinLocked, AlertTypeEnum.BinLocked },
                    { reading => reading.BinTilted, AlertTypeEnum.BinTilted },
                    { reading => reading.ServiceDoorOpen, AlertTypeEnum.ServiceDoorStuckOpen },
                    { reading => reading.FlapStuckOpen, AlertTypeEnum.FlapStuckOpen },
                    { reading => reading.BinFull, AlertTypeEnum.BinFull },
                };

            foreach (var condition in alertConditions)
            {
                if (condition.Key(unitReading))
                {
                    var alertForAdd = allPossibleAlertTypes.FirstOrDefault(a => a.Id == (int)condition.Value);
                    if (alertForAdd != null)
                    {
                        alertTypesForSend.Add(alertForAdd);
                    }
                }
            }

            // Check if the unit reading's create date is older than 24 hours
            if (unitReading.CreateDate < DateTime.Now.AddHours(-24) && includeDeviceNotReporting)
                alertTypesForSend.Add(allPossibleAlertTypes.First(at => at.Id == (int)AlertTypeEnum.DeviceNotReporting));
            
            return alertTypesForSend;
        }

        public static bool HasAnyAlert(this UnitReading unitReading)
        {
            // Definirajte alert uvjete
            var alertConditions = new List<Func<UnitReading, bool>>
               {
                   reading => reading.BatteryUvlo,
                   reading => reading.BinEmptiedLastPeriod,
                   reading => reading.BatteryOverTempLo,
                   reading => reading.BinLocked,
                   reading => reading.BinTilted,
                   reading => reading.ServiceDoorOpen,
                   reading => reading.FlapStuckOpen,
                   reading => reading.BinFull,
                   reading => reading.RegularPeriodicReporting == false
               };

            // Provjerite postoji li bilo koji alert uvjet
            return alertConditions.Any(condition => condition(unitReading));
        }

    }
}

