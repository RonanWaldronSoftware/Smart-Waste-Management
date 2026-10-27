using System.Security.Cryptography;
using System.Text;

namespace SmartWaste.Model.Extensions
{
    public static class StringExtensions
    {
        public static string ToSHA1Hash(this string inputString)
        {
            // Create byte from input string
            SHA1 sha1 = SHA1.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha1.ComputeHash(bytes);

            // Crete hash from bytes
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }

            // Return hash value
            return result.ToString().ToLower();
        }
        
        public static string ToMD5Hash(this string inputString)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

        public static string ToSHA512Hash(this string inputString)
        {
            // Create byte from input string
            SHA512 sha512 = SHA512.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha512.ComputeHash(bytes);

            // Crete hash from bytes
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }

            // Return hash value
            return result.ToString().ToLower();
        }
        /// <summary>
        /// Uses a regular expression to replace all characters that are not digits with an empty string.
        /// Uses a regular expression to remove leading zeros, except when the string is entirely zeros.
        /// </summary>
        /// <param name="phoneNo"></param>
        /// <returns></returns>
        public static string StripPhoneNo(this string phoneNo)
        {
            // Remove all except digits
            string newPhoneNumber = System.Text.RegularExpressions.Regex.Replace(phoneNo, "[^0-9]", "");

            // Remove leading zeros
            newPhoneNumber = System.Text.RegularExpressions.Regex.Replace(newPhoneNumber, "^0+(?=.)", "");

            return newPhoneNumber;
        }

        public static string ToHexString(this string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            char[] chars = input.ToCharArray();
            string hex = string.Empty;

            foreach (char c in chars)
            {
                hex += ((int)c).ToString("x2");
            }

            return hex;
        }

        public static byte[] HexStringToByteArray(this string hex)
        {
            if (string.IsNullOrEmpty(hex))
                throw new ArgumentException("Hex string must not be null or empty.", nameof(hex));

            if (hex.Length % 2 != 0)
                throw new ArgumentException("Hex string must have an even length.", nameof(hex));

            int length = hex.Length / 2;
            byte[] bytes = new byte[length];

            for (int i = 0; i < length; i++)
            {
                string byteValue = hex.Substring(i * 2, 2);
                bytes[i] = Convert.ToByte(byteValue, 16);
            }

            return bytes;
        }

        public static string GetContentType(this string fileName)
        {
            string contentType;
            switch (Path.GetExtension(fileName).ToLowerInvariant())
            {
                case ".jpg":
                case ".jpeg":
                    contentType = "image/jpeg";
                    break;
                case ".png":
                    contentType = "image/png";
                    break;
                case ".gif":
                    contentType = "image/gif";
                    break;
                case ".pdf":
                    contentType = "application/pdf";
                    break;
                default:
                    contentType = "application/octet-stream"; // defaultni content type
                    break;
            }
            return contentType;
        }
    }
}
