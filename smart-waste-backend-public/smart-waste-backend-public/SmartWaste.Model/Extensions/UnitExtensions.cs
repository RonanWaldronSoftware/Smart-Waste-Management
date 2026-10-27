using SmartWaste.Model.Constants;
using SmartWaste.Model.Core;
using SmartWaste.Model.Enums;
using SmartWaste.Model.Helpers;

namespace SmartWaste.Model.Extensions
{
    public static class UnitExtensions
    {
        public static UnitFillLevel ComputeUnitFillLevelPercentages(this Unit unit, LatestUnitReading? unitReading)
        {
            int binLevelPercentage;
            int binLevelBcPercentage;
            double totalLiters = 0;

            if (unit.UnitTypeId == 1)
            {
                // Tekelek Sensor
                binLevelPercentage = ComputePercentageTekelek(unit.UnitModelTypeId.Value, unitReading?.BinLevel ?? 0);
                binLevelBcPercentage = ComputePercentageTekelek(unit.UnitModelTypeId.Value, unitReading?.BinLevelBc ?? 0);
            }
            else
            {
                // Pel Bin Sensor
                binLevelPercentage = ComputePercentagePelBin(unit.UnitModelTypeId.Value, unitReading?.BinLevel ?? 0);
                binLevelBcPercentage = ComputePercentagePelBin(unit.UnitModelTypeId.Value, unitReading?.BinLevelBc ?? 0);
            }

            // Calculate liters
            if (unit.UnitModelType is not null)
                totalLiters = CalculateLitersFromPercentage(binLevelPercentage, unit.UnitModelType.Capacity);

            return new UnitFillLevel
            {
                FillLevelPercentage = binLevelPercentage,
                FillLevelBcPercentage = binLevelBcPercentage,
                FillnessStatusId = DetermineBinLevelFillnessStatus(unit, binLevelPercentage),
                FillLevelInLiters = totalLiters,
            };
        }
        
        public static UnitFillLevel ComputeUnitFillLevelPercentages(this Unit unit, UnitReading? unitReading)
        {
            int binLevelPercentage;
            int binLevelBcPercentage;
            double totalLiters = 0;

            if (unit.UnitTypeId == 1)
            {
                // Tekelek Sensor
                binLevelPercentage = ComputePercentageTekelek(unit.UnitModelTypeId.Value, unitReading?.BinLevel ?? 0);
                binLevelBcPercentage = ComputePercentageTekelek(unit.UnitModelTypeId.Value, unitReading?.BinLevelBc ?? 0);
            }
            else
            {
                // Pel Bin Sensor
                binLevelPercentage = ComputePercentagePelBin(unit.UnitModelTypeId.Value, unitReading?.BinLevel ?? 0);
                binLevelBcPercentage = ComputePercentagePelBin(unit.UnitModelTypeId.Value, unitReading?.BinLevelBc ?? 0);
            }

            // Calculate liters
            if (unit.UnitModelType is not null)
                totalLiters = CalculateLitersFromPercentage(binLevelPercentage, unit.UnitModelType.Capacity);

            return new UnitFillLevel
            {
                FillLevelPercentage = binLevelPercentage,
                FillLevelBcPercentage = binLevelBcPercentage,
                FillnessStatusId = DetermineBinLevelFillnessStatus(unit, binLevelPercentage),
                FillLevelInLiters = totalLiters,
            };
        }

        public static double CalculateLitersFromPercentage(int binLevelPercentage, int modelCapacity)
        {
            return (modelCapacity / 100) * binLevelPercentage;
        }

        private static int DetermineBinLevelFillnessStatus(Unit unit, int binLevelPercentage)
        {
            if (binLevelPercentage < unit.EmptyLevel)
            {
                return DatabaseConstants.UnitFullnessStatus.Small.Id;
            }
            else if (binLevelPercentage >= unit.EmptyLevel && binLevelPercentage <= unit.FullLevel)
            {
                return DatabaseConstants.UnitFullnessStatus.Medium.Id;
            }
            else // if (unitReading.BinLevel > unit.FullLevel)
            {
                return DatabaseConstants.UnitFullnessStatus.Critical.Id;
            }
        }

        private static int ComputePercentageTekelek(int unitModelType, int value)
        {
            int result = 0;

            double reading40percent = 0;
            double reading100percent = 0;

            if (unitModelType == (int)UnitModelTypeEnum.Model120 || unitModelType == (int)UnitModelTypeEnum.Model170)
            {
                reading40percent = 72.0;
                reading100percent = 18.0;
            }
            else
            {
                // Model 240, Model 360, Model 600
                reading40percent = 73.9;
                reading100percent = 18.0;
            }

            double dominator = (reading40percent - reading100percent) / 60;
            result = (int)Math.Round(100 - (value - reading100percent) / dominator);

            // Negative readings are wrong so for readability, we will display them as 0 %
            return FixResult(result);

        }

        private static int ComputePercentagePelBin(int unitModelType, int value)
        {
            int result = 0;

            double readingZeropercent = 0;
            double reading40percent = 0;
            double reading50percent = 0;
            double reading70percent = 0;
            double reading100percent = 0;

            if (unitModelType == (int)UnitModelTypeEnum.Model120 || unitModelType == (int)UnitModelTypeEnum.Model170)
            {
                // Model 120 (binType = 1) or Model 170 (binType = 5)
                // uses reading at 50% to interpolate readings above and below 50%
                readingZeropercent = 156;
                reading50percent = 53;
                reading100percent = 29;

                if (value >= reading50percent)
                    result = (int)Math.Round(50 * (value - reading50percent) / (readingZeropercent - reading50percent)) + 50;
                else
                    result = (int)Math.Round(50 * (value - reading100percent) / (reading50percent - reading100percent));
            }
            // Only for model 165
            else if (unitModelType == (int)UnitModelTypeEnum.Model165)
            {
                readingZeropercent = 28;
                reading70percent = 50;
                reading100percent = 93;

                if (value >= reading70percent)
                    result = (int)Math.Round(30 * (value - reading70percent) / (reading100percent - reading70percent)) + 70;
                else
                    result = (int)Math.Round(70 * (value - readingZeropercent) / (reading70percent - readingZeropercent));
            }
            // Only for model 150
            else if (unitModelType == (int)UnitModelTypeEnum.Model150)
            {
                readingZeropercent = 42;
                reading40percent = 70;
                reading100percent = 87;

                if (value >= reading70percent)
                    result = (int)Math.Round(60 * (value - reading40percent) / (reading100percent - reading40percent)) + 40;
                else
                    result = (int)Math.Round(40 * (value - readingZeropercent) / (reading40percent - readingZeropercent));
            }
            // Fila Superior - gap 37cm, fill height 27cm
            else if (unitModelType == (int)UnitModelTypeEnum.FilaSuperior)
            {
                double fillGap = 37;
                double fillHeight = 27;
                readingZeropercent = fillGap + fillHeight; // 64
                reading100percent = fillGap; // 37

                result = (int)Math.Round(100 * (readingZeropercent - value) / (readingZeropercent - reading100percent));
            }
            // Fila Inferior - gap 31cm, fill height 27cm
            else if (unitModelType == (int)UnitModelTypeEnum.FilaInferior)
            {
                double fillGap = 31;
                double fillHeight = 27;
                readingZeropercent = fillGap + fillHeight; // 58
                reading100percent = fillGap; // 31

                result = (int)Math.Round(100 * (readingZeropercent - value) / (readingZeropercent - reading100percent));
            }
            else
            {
                // Model 240, Model 360  - uses reading at 40% to interpolate readings above and below 40%
                readingZeropercent = 140;
                reading40percent = 42;
                reading100percent = 21;

                if (value >= reading40percent)
                    result = (int)Math.Round(60 * (value - reading40percent) / (readingZeropercent - reading40percent)) + 40;
                else
                    result = (int)Math.Round(40 * (value - reading100percent) / (reading40percent - reading100percent));
            }

            // Negative readings are wrong so for readability, we will display them as 0 %
            return FixResult(result);
        }

        /// <summary>
        /// This method  ensures that the input value (result) is constrained within the range of 0 to 100. 
        /// If the value is less than 0, it returns 0. If the value is greater than 100, it returns 100. Otherwise, it returns the original value.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private static int FixResult(int result)
        {
            if (result < 0)
                return 0;

            else if (result > 100)
                return 100;

            return result;
        }
    }
}
