using AutoMapper;
using SmartWaste.Model.Core;
using SmartWaste.Repository;
using SmartWaste.Model.Extensions;
using Newtonsoft.Json;
using SmartWaste.Model.Constants;
using SmartWaste.Model.Helpers.RabbitMQ;
using SmartWaste.Service;

namespace SmartWaste.Handler.Service
{
    public static class UnitStaticService
    {
        private const int MaxMessageLength = 30;
        public static async Task<UnitReading?> ProcessSmartWasteData(DataSensorModel dataSensor, IRepositoryManager repositoryManager, ILogger<Controllers.UnitHandlerController> _logger,
            IMapper _mapper, IServiceManager _serviceManager)
        {
            try
            {
                // Set data reference
                var data = dataSensor.Data;

                UnitReading reading = new UnitReading
                {
                    MessageType = GetMessageType(dataSensor),
                    FullnessStatusId = DatabaseConstants.UnitFullnessStatus.Small.Id, // Default status
                };

                // Set source
                reading.Source = GetMessageTypeSource(reading.MessageType ?? 0);

                int idSize;
                Unit unit;

                switch (reading.MessageType)
                {
                    case 1:
                        reading.BinLevelBc = data[1] & 0xff;
                        reading.BinLevel = data[2] & 0xff;
                        reading.NoFlapOpenings = (data[3] & 0xff) * 256 + (data[4] & 0xff);
                        reading.BatteryVoltage = data[5] & 0xff;
                        reading.Temperature = (sbyte)data[6];
                        reading.NoCompactions = data[7] & 0xff;

                        // Check if bin level is 0
                        if (reading.BinLevel == 0)
                            reading.BinLevel = reading.BinLevelBc;

                        int flags = data[8] & 0xff;
                        reading.BatteryUvlo = (flags & 0x01) == 0x01;
                        reading.BinEmptiedLastPeriod = (flags & 0x02) == 0x02;
                        reading.BatteryOverTempLo = (flags & 0x04) == 0x04;
                        reading.BinLocked = (flags & 0x08) == 0x08;
                        reading.BinFull = (flags & 0x10) == 0x10;
                        reading.BinTilted = (flags & 0x20) == 0x20;
                        reading.ServiceDoorClosed = (flags & 0x40) == 0x40;
                        reading.FlapStuckOpen = (flags & 0x80) == 0x80;

                        flags = data[9] & 0xff;
                        reading.ServiceDoorOpen = (flags & 0x01) == 0x01;
                        reading.NblotSignalStrength = 0;
                        reading.Rsrq = (-19.5 + 0.5 * (data[10] & 0xff));
                        reading.Rsrp = -140 + (data[11] & 0xff);

                        idSize = data[12];

                        if (data.Length > MaxMessageLength)
                        {
                            _logger.LogError($"SmartWaste messages are 30 bytes or less - this message is {data.Length} bytes long");
                            throw new Exception($"SmartWaste messages are 30 bytes or less - this message is {data.Length} bytes long");
                        }
                        else if (data.Length < (13 + idSize))
                        {
                            _logger.LogError($"There is not enough bytes in the message for a serialNo of size {idSize} bytes.");
                            throw new Exception($"There is not enough bytes in the message for a serialNo of size {idSize} bytes.");
                        }

                        reading.SerialNumber = string.Concat(Enumerable.Range(13, idSize).Select(i => data[i].ByteToHex())).TrimStart('0').ToUpper();

                        int i = 0;
                        while (i < reading.SerialNumber.ToString().Length - 1 && reading.SerialNumber.ToString()[i] == '0')
                            i++;
                        reading.SerialNumber = reading.SerialNumber.ToString().Substring(i).ToUpper().Trim();

                        if (string.IsNullOrEmpty(reading.SerialNumber))
                            break;

                        unit = await repositoryManager.Unit.GetUnit(reading.SerialNumber, false);

                        reading.ReadingDate = DateTime.UtcNow;

                        if (unit is not null)
                        {
                            reading.Firmware = unit.FirmWare;
                            reading.TimeDiff = unit.TimeDifference;
                            reading.UnitJustOn = unit.UnitJustOn;
                            reading.RegularPeriodicReporting = unit.RegularPeriodicReporting;
                            reading.NbloTissue = unit.NbloTissue;
                            reading.UnitId = unit.Id;
                            reading.FullnessStatusId = unit.ComputeUnitFillLevelPercentages(reading).FillnessStatusId;
                            reading.CreateDate = DateTime.UtcNow;

                            // Calculate latest unit reading and save into database
                            var latestUnitReading = _mapper.Map<LatestUnitReading>(reading);
                            latestUnitReading.BinLevelPercentage = unit.ComputeUnitFillLevelPercentages(reading).FillLevelPercentage;
                            await _serviceManager.UnitReadingService.AddLatestUnitReading(latestUnitReading);
                        }

                        using (_logger.BeginScope(("full_message", JsonConvert.SerializeObject(reading))))
                        {
                            _logger.LogInformation("Creating new unit reading SmartWaste");
                        }

                        reading = await repositoryManager.UnitReading.CreateReading(reading);

                        _logger.LogInformation($"Unit reading saved successfuly for SmartWaste unit {unit?.Id}");
                        break;
                    case 5:
                        reading.Firmware = $"{data[1] & 0xff:D2}-{data[2] & 0xff:D2}-{data[3] & 0xff:D2} {data[4] & 0xff:D2}:{data[5] & 0xff:D2}:{data[6] & 0xff:D2}";

                        try
                        {
                            int hour = data[7] & 0xff;
                            int minute = data[8] & 0xff;
                            int second = data[9] & 0xff;

                            DateTime now = DateTime.UtcNow;
                            DateTime unitTime = new DateTime(now.Year, now.Month, now.Day, hour, minute, second, DateTimeKind.Utc);
                            reading.TimeDiff = (int)(unitTime - now).TotalSeconds;
                        }
                        catch (Exception ex)
                        {
                            reading.TimeDiff = 0;
                        }

                        int flags5 = data[10] & 0xff;
                        reading.UnitJustOn = (flags5 & 0x01) == 0x01;
                        reading.RegularPeriodicReporting = (flags5 & 0x02) == 0x02;
                        reading.NbloTissue = (flags5 & 0x04) == 0x04;
                        reading.HardwareRevision = data[11] & 0xff;

                        idSize = data[12];

                        if (data.Length > MaxMessageLength)
                        {
                            _logger.LogError($"SmartWaste messages are 30 bytes or less - this message is {data.Length} bytes long");
                            throw new Exception($"SmartWaste messages are 30 bytes or less - this message is {data.Length} bytes long");
                        }
                        else if (data.Length < (13 + idSize))
                        {
                            _logger.LogError($"There is not enough bytes in the message for a serialNo of size {idSize} bytes.");
                            throw new Exception($"There is not enough bytes in the message for a serialNo of size {idSize} bytes.");
                        }

                        reading.SerialNumber = string.Concat(Enumerable.Range(13, idSize).Select(j => data[j].ByteToHex())).TrimStart('0').ToUpper();

                        int j = 0;
                        while (j < reading.SerialNumber.ToString().Length - 1 && reading.SerialNumber.ToString()[j] == '0')
                            j++;
                        reading.SerialNumber = reading.SerialNumber.ToString().Substring(j).ToUpper().Trim();

                        if (string.IsNullOrEmpty(reading.SerialNumber))
                            break;

                        unit = await repositoryManager.Unit.GetUnit(reading.SerialNumber, false);

                        if (unit is not null)
                        {
                            var latestUnitReading = await repositoryManager.UnitReading.GetLatestUnitReadingFromLatestUnitReading(unit.Id, false);

                            // Set last reading values to new reading
                            if (latestUnitReading is not null)
                            {
                                reading.BatteryVoltage = latestUnitReading.BatteryVoltage;
                                reading.Temperature = latestUnitReading.Temperature;
                                reading.BinLevel = latestUnitReading.BinLevel;
                                reading.BinLevelBc = latestUnitReading.BinLevelBc;
                                reading.NoFlapOpenings = latestUnitReading.NoFlapOpenings;
                                reading.NoCompactions = latestUnitReading.NoCompactions;
                            }

                            unit.FirmWare = reading.Firmware;
                            reading.UnitId = unit.Id;
                            reading.ReadingDate = DateTime.UtcNow;
                            reading.CreateDate = DateTime.UtcNow;
                            reading.FullnessStatusId = unit.ComputeUnitFillLevelPercentages(reading).FillnessStatusId;

                            // Calculate latest unit reading and save into database
                            latestUnitReading = _mapper.Map<LatestUnitReading>(reading);
                            latestUnitReading.BinLevelPercentage = unit.ComputeUnitFillLevelPercentages(reading).FillLevelPercentage;
                            await _serviceManager.UnitReadingService.AddLatestUnitReading(latestUnitReading);
                        }

                        using (_logger.BeginScope(("full_message", JsonConvert.SerializeObject(reading))))
                        {
                            _logger.LogInformation("Creating new unit reading SmartWaste");
                        }

                        reading = await repositoryManager.UnitReading.CreateReading(reading);

                        _logger.LogInformation($"Unit reading saved successfuly for SmartWaste unit {unit?.Id}");
                        break;

                    // Tekelek case
                    case 4:
                    case 6:
                    case 8:
                        // SmartWaste Type 6 - GNSS location message
                        if (reading.MessageType == 6 && dataSensor.Sensor == SensorEnum.SmartWaste)
                        {
                            if (data.Length < 10)
                            {
                                _logger.LogError("Type 6 message too short to contain latitude, longitude and serial number length");
                                throw new Exception("Type 6 message too short");
                            }

                            float latitude = BitConverter.ToSingle(data, 1);
                            float longitude = BitConverter.ToSingle(data, 5);

                            idSize = data[9];

                            if (data.Length < (10 + idSize))
                            {
                                _logger.LogError($"There is not enough bytes in the message for a serialNo of size {idSize} bytes.");
                                throw new Exception($"There is not enough bytes in the message for a serialNo of size {idSize} bytes.");
                            }

                            reading.SerialNumber = string.Concat(Enumerable.Range(10, idSize).Select(k => data[k].ByteToHex())).TrimStart('0').ToUpper();

                            if (string.IsNullOrEmpty(reading.SerialNumber))
                                break;

                            unit = await repositoryManager.Unit.GetUnit(reading.SerialNumber, trackChanges: true);

                            if (unit is not null)
                            {
                                unit.Latitude = (decimal)latitude;
                                unit.Longitude = (decimal)longitude;
                                unit.UpdateDate = DateTime.UtcNow;
                                await repositoryManager.SaveAsync();

                                _logger.LogInformation($"Unit {unit.Id} GNSS location updated: lat={latitude}, lng={longitude}");
                            }
                            else
                            {
                                _logger.LogWarning($"Type 6 GNSS message received for unknown unit with serial number {reading.SerialNumber}");
                            }

                            break;
                        }
                        var readings = new List<UnitReading>();

                        int startByte = 25;
                        int bytesPerReading = 4;

                        var serialNumber = data.ExtractSerialNumber();

                        if (string.IsNullOrEmpty(serialNumber))
                            break;

                        unit = await repositoryManager.Unit.GetUnit(serialNumber, false);
                        
                        int loggerSpeed = data[23] == 80 ? 15 : (data[23] & 0x7f) * 15;

                        int baseTimeHours = (data[19] & 0x1f);
                        int baseTimeMinutes = data[25];

                        DateTime today = DateTime.Today;
                        DateTime baseDateTime = today.AddHours(baseTimeHours).AddMinutes(baseTimeMinutes);

                        for (int z = 0; z < (data.Length - startByte) / bytesPerReading; z++)
                        {
                            // Get bits for check
                            string bits = $"{data[26 + (z * 4)]}{data[27 + (z * 4)]}{data[28 + (z * 4)]}{data[29 + (z * 4)]}";

                            // Check if bits are valid
                            if (bits.All(c => c.Equals('0')))
                                break;

                            UnitReading unitReading = new UnitReading
                            {
                                MessageType = GetMessageType(dataSensor),
                                FullnessStatusId = DatabaseConstants.UnitFullnessStatus.Small.Id, // Default status              
                            };

                            // Set source
                            unitReading.Source = GetMessageTypeSource(unitReading.MessageType ?? 0);

                            double batteryVoltageTek = (30 + (data[6] & 0x1F)) / 10.0f;

                            unitReading.BinLevel = ((data[28 + (z * 4)] >> 6) & 0x03) << 8 | data[29 + (z * 4)];
                            unitReading.Temperature = (data[27 + (z * 4)] / 2) - 30;
                            unitReading.SerialNumber = serialNumber;
                            unitReading.ReadingDate = baseDateTime.AddMinutes(-loggerSpeed * z);
                            unitReading.NoFlapOpenings = 0;
                            unitReading.NoCompactions = 0;
                            unitReading.NblotSignalStrength = 0;
                            unitReading.BatteryVoltage = (int)Math.Round(((batteryVoltageTek - 7) / 0.05), 0);
                            unitReading.BinLevelBc = 0;

                            if (string.IsNullOrEmpty(unitReading.SerialNumber))
                                break;

                            using (_logger.BeginScope(("full_message", JsonConvert.SerializeObject(unitReading))))
                            {
                                _logger.LogInformation("Creating new unit reading tekelek");
                            }

                            // Check if unit id is same
                            if (unit is not null)
                            {
                                unitReading.UnitId = unit.Id;
                                unitReading.CreateDate = DateTime.UtcNow;
                                unitReading.FullnessStatusId = unit.ComputeUnitFillLevelPercentages(unitReading).FillnessStatusId;
                            }

                            unitReading = await repositoryManager.UnitReading.CreateReading(unitReading);

                            readings.Add(unitReading);

                            _logger.LogInformation($"Unit reading saved successfully for tekelek unit {unit?.Id}");
                        }

                        reading = readings.First();

                        if (unit is not null)
                        {
                            // Calculate latest unit reading and save into database
                            var latestUnitReading = _mapper.Map<LatestUnitReading>(reading);
                            latestUnitReading.BinLevelPercentage = unit.ComputeUnitFillLevelPercentages(reading).FillLevelPercentage;
                            await _serviceManager.UnitReadingService.AddLatestUnitReading(latestUnitReading);
                        }

                        break;
                    default:
                        throw new Exception($"UnitServices.SaveUnitReading: Unknown message type msgType: {reading.MessageType}");
                }

                return reading;
            }
            catch (Exception ex)
            {
                using (_logger.BeginScope(("full_message", JsonConvert.SerializeObject(ex.ToString()))))
                {
                    _logger.LogInformation($"Error while proccessing unit rading data");
                }

                return null;
            }
        }

        public static async Task<UnitReading?> ProcessSigfoxData(SigFoxData sigfoxData, IRepositoryManager repositoryManager, ILogger<Controllers.UnitHandlerController> _logger,
            IMapper _mapper, IServiceManager _serviceManager)
        {
            // Strip leading zeros from deviceID and serialNo
            sigfoxData.SerialNumber = sigfoxData.SerialNumber.TrimStart('0').Trim();
            sigfoxData.SigfoxBody.DeviceID = sigfoxData.SigfoxBody.DeviceID.TrimStart('0').Trim();

            if (!string.Equals(sigfoxData.SigfoxBody.DeviceID, sigfoxData.SerialNumber, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogError("Sigfox process API parameter id does NOT match body deviceID");
                throw new Exception("API parameter id does NOT match body deviceID");
            }

            UnitReading reading = new UnitReading
            {
                SerialNumber = sigfoxData.SigfoxBody.DeviceID,
                FullnessStatusId = DatabaseConstants.UnitFullnessStatus.Small.Id, // Default status
                Source = "Sigfox",
            };

            // Parse RSSI and SNR values
            reading.Rssi = (decimal?)(double.TryParse(sigfoxData.SigfoxBody.Rssi, out var rssi) ? rssi : 0.0);
            reading.Snr = double.TryParse(sigfoxData.SigfoxBody.Snr, out var snr) ? snr : 0.0;

            // Convert data from hex string to byte array
            byte[] data = sigfoxData.SigfoxBody.Data.HexStringToByteArray();

            // Determine the message type
            reading.MessageType = data[0];

            // Get unit by serial number
            Unit unit = await repositoryManager.Unit.GetUnit(sigfoxData.SigfoxBody.DeviceID, isSigfox: true);

            switch (reading.MessageType)
            {
                case 1:  // Message Type 1
                    reading.BinLevelBc = data[1];
                    reading.BinLevel = data[2];
                    reading.NoFlapOpenings = (data[3] << 8) + data[4];
                    reading.BatteryVoltage = data[5];
                    reading.Temperature = (sbyte)data[6];  // signed value
                    reading.NoCompactions = data[7];

                    // Check if bin level is 0
                    if (reading.BinLevel == 0)
                        reading.BinLevel = reading.BinLevelBc;

                    int flags = data[8];
                    reading.BatteryUvlo = (flags & 0x01) == 0x01;
                    reading.BinEmptiedLastPeriod = (flags & 0x02) == 0x02;
                    reading.BatteryOverTempLo = (flags & 0x04) == 0x04;
                    reading.BinLocked = (flags & 0x08) == 0x08;
                    reading.BinFull = (flags & 0x10) == 0x10;
                    reading.BinTilted = (flags & 0x20) == 0x20;
                    reading.ServiceDoorClosed = (flags & 0x40) == 0x40;
                    reading.FlapStuckOpen = (flags & 0x80) == 0x80;

                    flags = data[9];
                    reading.ServiceDoorOpen = (flags & 0x01) == 0x01;
                    reading.NblotSignalStrength = 0; // Not used in this example
                    reading.ReadingDate = DateTime.UtcNow;

                    if (unit is not null)
                    {
                        // Set firmware parameters in the reading
                        reading.Firmware = unit.FirmWare;
                        reading.TimeDiff = unit.TimeDifference;
                        reading.UnitJustOn = unit.UnitJustOn;
                        reading.RegularPeriodicReporting = unit.RegularPeriodicReporting;
                        reading.NbloTissue = unit.NbloTissue;
                        reading.UnitId = unit.Id;
                        reading.CreateDate = DateTime.UtcNow;

                        // Calculate latest unit reading and save into database
                        var latestUnitReading = _mapper.Map<LatestUnitReading>(reading);
                        latestUnitReading.BinLevelPercentage = unit.ComputeUnitFillLevelPercentages(reading).FillLevelPercentage;
                        await _serviceManager.UnitReadingService.AddLatestUnitReading(latestUnitReading);
                    }

                    using (_logger.BeginScope(("full_message", JsonConvert.SerializeObject(reading))))
                    {
                        _logger.LogInformation("Creating new unit reading from sigfox data");
                    }

                    reading = await repositoryManager.UnitReading.CreateReading(reading);

                    _logger.LogInformation($"Unit reading saved successfuly for sigfox unit {unit?.Id}");

                    break;
                case 5:  // Message Type 5
                    _logger.LogDebug("MessageType: 5");

                    string firmware = $"{data[1]:D2}-{data[2]:D2}-{data[3]:D2} {data[4]:D2}:{data[5]:D2}:{data[6]:D2}";
                    reading.Firmware = firmware;

                    int flags5 = data[10];
                    reading.UnitJustOn = (flags5 & 0x80) == 0x01;
                    reading.RegularPeriodicReporting = (flags5 & 0x02) == 0x02;
                    reading.NbloTissue = (flags5 & 0x04) == 0x04;

                    reading.HardwareRevision = data[11];
                    reading.ReadingDate = DateTime.UtcNow;

                    try
                    {
                        int hour = data[7];
                        int minute = data[8];
                        int second = data[9];

                        DateTime now = DateTime.UtcNow;
                        DateTime unitTime = new DateTime(now.Year, now.Month, now.Day, hour, minute, second, DateTimeKind.Utc);
                        var timeDiff = Math.Abs((unitTime - now).TotalSeconds);

                        reading.TimeDiff = (int?)timeDiff;

                        if (!reading.UnitJustOn && timeDiff > 300)
                        {
                            _logger.LogDebug($"Save message type 4 - to sync unit with server time - timeDiff (sec's): {timeDiff}");

                            byte[] msgData = new byte[] {
                            0x04, // Message Type = 4 - set time on the unit
                            0x00, // year = 00
                            0x00, // month = 00
                            0x00, // day = 00
                            0x00, // hours = 00
                            0x00, // minutes = 00
                            0x00, // seconds = 00
                            0x00  // Flags - all set to ignore
                            };
                        }

                        if (unit is not null)
                        {
                            var latestUnitReading = await repositoryManager.UnitReading.GetLatestUnitReadingFromLatestUnitReading(unit.Id, false);

                            // Set last reading values to new reading
                            if (latestUnitReading is not null)
                            {
                                reading.BatteryVoltage = latestUnitReading.BatteryVoltage;
                                reading.Temperature = latestUnitReading.Temperature;
                                reading.BinLevel = latestUnitReading.BinLevel;
                                reading.BinLevelBc = latestUnitReading.BinLevelBc;
                                reading.NoFlapOpenings = latestUnitReading.NoFlapOpenings;
                                reading.NoCompactions = latestUnitReading.NoCompactions;
                            }

                            unit.FirmWare = reading.Firmware;
                            reading.UnitId = unit.Id;
                            reading.ReadingDate = DateTime.UtcNow;
                            reading.CreateDate = DateTime.UtcNow;
                            reading.FullnessStatusId = unit.ComputeUnitFillLevelPercentages(reading).FillnessStatusId;

                            // Calculate latest unit reading and save into database
                            latestUnitReading = _mapper.Map<LatestUnitReading>(reading);
                            latestUnitReading.BinLevelPercentage = unit.ComputeUnitFillLevelPercentages(reading).FillLevelPercentage;
                            await _serviceManager.UnitReadingService.AddLatestUnitReading(latestUnitReading);
                        }

                        using (_logger.BeginScope(("full_message", JsonConvert.SerializeObject(reading))))
                        {
                            _logger.LogInformation("Creating new unit reading from sigfox data");
                        }

                        reading = await repositoryManager.UnitReading.CreateReading(reading);

                        _logger.LogInformation($"Unit reading saved successfuly for sigfox unit {unit?.Id}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"ERROR: Message Type = 5: Converting device time to valid time failed {ex.Message}");
                        reading.TimeDiff = 0;
                    }

                    break;

                case 6:  // Message Type 6 (This is to match a sigfoxId with a unit with a EUI-48 ID)
                    int idLength = data[1];
                    string eui48 = sigfoxData.SigfoxBody.Data.Substring(4, idLength * 2);

                    _logger.LogInformation($"Process sigfoxData message Type 6: sigfoxId: {sigfoxData.SigfoxBody.DeviceID} EUI-48: {eui48}");

                    if (unit is not null)
                    {
                        unit.SerialNumber = eui48.TrimStart('0');
                        unit.SigFoxAltId = sigfoxData.SigfoxBody.DeviceID;

                        await repositoryManager.SaveAsync();
                    }
                    break;
                default:
                    _logger.LogError($"Sigfox process data Unknown message type MsgType: {reading.MessageType}");
                    throw new Exception($"SigfoxService.ProcessData: Unknown message type MsgType: {reading.MessageType}");
            }

            _logger.LogInformation("Process sigfoxData - end");

            return reading;
        }


        /// <summary>
        /// Retrieves the message type based on the sensor type.
        /// - For SmartWaste sensor, returns the value of the first byte.
        /// - For other sensors, returns the lower 6 bits from the byte at index 15.
        /// </summary>
        /// <param name="dataSensor"></param>
        /// <returns></returns>
        private static int GetMessageType(DataSensorModel dataSensor)
        {
            if (dataSensor.Sensor == SensorEnum.SmartWaste)
            {
                return dataSensor.Data[0] & 0xff;
            }

            return dataSensor.Data[15] & 0b00111111;
        }
        private static string GetMessageTypeSource(int messageType)
        {
            return messageType switch
            {
                4 => "NB-IoT Tek",
                7 => "NB-IoT Tek",
                8 => "NB-IoT Tek",
                _ => "NB-IoT BB"
            };
        }
    }
}
