namespace SmartWaste.Model.Extensions
{
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Extracts a serial number from a byte array that begins with the byte 0x08.
        /// </summary>
        /// <param name="byteArray">The byte array from which the serial number is extracted.</param>
        /// <param name="serialNumberLength">The length of the serial number in bytes.</param>
        /// <returns>The serial number as a string, or null if the serial number is not found or there is insufficient data.</returns>
        public static string ExtractSerialNumber(this byte[] byteArray, int serialNumberLength = 16)
        {
            // Find the index of the first byte with value 0x08
            int startIndex = Array.IndexOf(byteArray, (byte)0x08);
            if (startIndex < 0 || startIndex + serialNumberLength > byteArray.Length)
                return null; // Serial number not found or insufficient data
            
            // Extract the serial number from the byte array
            byte[] serialNumberBytes = byteArray.Skip(startIndex).Take(serialNumberLength).ToArray();
            string serialNumber = BitConverter.ToString(serialNumberBytes).Replace("-", "");
            return serialNumber[..16].Trim();
        }
    }
}

