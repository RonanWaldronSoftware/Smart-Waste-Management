namespace SmartWaste.Model.Extensions
{
    public static class EnumerableExtensions
    {
        public static List<int> ToIdList<T>(this IEnumerable<T> source)
        {
            return source.Select(item => (int)item.GetType().GetProperty("Id").GetValue(item)).ToList();
        }
    }
}
