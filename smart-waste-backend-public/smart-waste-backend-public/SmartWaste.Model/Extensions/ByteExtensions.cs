namespace SmartWaste.Model.Extensions
{
    public static class ByteExtensions
    {
        public static string ByteToHex(this byte b)
        {
            return b.ToString("X2");
        }
    }
}
