using BankDirectoryApi.Domain.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BankDirectoryApi.Common.Helpers
{
    /// <summary>
    /// Helper class for creating orderings based on provided information.
    /// </summary>
    public static class OrderingHelper
    {
        /// <summary>
        /// Creates a list of orderings based on the provided ordering information.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orderingInfo"></param>
        /// <returns> A list of orderings.</returns>
        public static List<Ordering<T>> GetOrderings<T>(Dictionary<string, string> orderingInfo)
        {
            var orderings = new List<Ordering<T>>();
            if (orderingInfo == null || orderingInfo.Count == 0)
            {
                return orderings;
            }
            foreach (var order in orderingInfo)
            {
                var propertyName = order.Key;
                var isDescending = order.Value.Equals("desc", StringComparison.OrdinalIgnoreCase);

                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.PropertyOrField(parameter, propertyName);
                var lambda = Expression.Lambda<Func<T, object>>(Expression.Convert(property, typeof(object)), parameter);

                orderings.Add(new Ordering<T> { OrderBy = lambda, IsDescending = isDescending });
            }

            return orderings;
        }
    }

 
}
