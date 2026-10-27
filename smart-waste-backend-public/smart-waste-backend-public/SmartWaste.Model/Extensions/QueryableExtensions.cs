using System.Linq.Expressions;

namespace SmartWaste.Model.Extensions
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Extension for database order by dynamic
        /// </summary>
        /// <returns>The by dynamic.</returns>
        /// <param name="query">Query.</param>
        /// <param name="orderByMember">Order by member.</param>
        /// <param name="direction">Direction.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        private static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string orderByMember, string direction)
        {
            // Check if order by member is empty
            if (string.IsNullOrEmpty(orderByMember))
                return query;
            
            var orderByProps = orderByMember.Split('.');
            var queryElementTypeParam = Expression.Parameter(typeof(T));

            var orderByProperty = orderByProps.Aggregate<string?, Expression>(queryElementTypeParam, Expression.PropertyOrField!);

            var keySelector = Expression.Lambda(orderByProperty, queryElementTypeParam);

            var orderBy = Expression.Call(
                typeof(Queryable),
                direction.Equals("asc") ? "OrderBy" : "OrderByDescending",
                new[] { typeof(T), orderByProperty.Type },
                query.Expression,
                Expression.Quote(keySelector));

            return query.Provider.CreateQuery<T>(orderBy);
        }


        /// <summary>
        /// Order query by dinamic data and get page
        /// </summary>
        /// <returns>The by page.</returns>
        /// <param name="query">Query.</param>
        /// <param name="orderByMember">Order by member.</param>
        /// <param name="direction">Direction.</param>
        /// <param name="page">Page.</param>
        /// <param name="pageSize">Page size.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static IQueryable<T> OrderByPage<T>(this IQueryable<T> query, string orderByMember, string direction, int page, int pageSize)
        {
            // Calculate skip
            var skip = (page - 1) * pageSize;

            return query.OrderByDynamic(orderByMember, direction).Skip(skip).Take(pageSize);
        }
    }

}
