using BankDirectoryApi.Domain.Attributes;
using BankDirectoryApi.Domain.Classes.Specifications;
using BankDirectoryApi.Domain.Enums;
using BankDirectoryApi.Domain.Interfaces.Specifications;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Common.Helpers
{
    public static class ExpressionFilterHelper
    {
        public static Expression<Func<T, bool>> CreateFilter<T>(object filterObject)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var conditions = new List<Expression>();

            foreach (var dtoProperty in filterObject.GetType().GetProperties())
            {
                var value = dtoProperty.GetValue(filterObject);
                if (value == null) continue;

                var filterAttr = dtoProperty.GetCustomAttribute<FilterAttribute>();
                var mappedName = filterAttr?.MappedProperty ?? dtoProperty.Name;

                var entityProperty = typeof(T).GetProperty(mappedName);
                if (entityProperty == null) continue;

                var property = Expression.Property(parameter, entityProperty);
                var constant = Expression.Constant(value);
                Expression condition = Expression.Empty();
                if (dtoProperty.PropertyType == typeof(string) && filterAttr != null)
                {
                    MethodInfo? stringMethod = filterAttr.Type switch
                    {
                        FilterType.Contains => typeof(string).GetMethod("Contains", new[] { typeof(string) }),
                        FilterType.StartsWith => typeof(string).GetMethod("StartsWith", new[] { typeof(string) }),
                        FilterType.EndsWith => typeof(string).GetMethod("EndsWith", new[] { typeof(string) }),
                        _ => null
                    };

                    if (stringMethod != null)
                    {
                        var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                        var propertyLower = Expression.Call(property, toLowerMethod!);
                        var valueLower = Expression.Call(constant, toLowerMethod!);
                        condition = Expression.Call(propertyLower, stringMethod, valueLower);
                    }
                    else
                    {
                        condition = Expression.Equal(property, constant);
                    }
                }
                else if (Nullable.GetUnderlyingType(dtoProperty.PropertyType) != null)
                {
                    var hasValueProperty = Expression.Property(property, "HasValue");
                    var valueProperty = Expression.Property(property, "Value");
                    condition = Expression.AndAlso(hasValueProperty, Expression.Equal(valueProperty, constant));
                }

                switch (filterAttr?.Type)
                    {
                     
                        case FilterType.GreaterThan:
                            {
                            condition = Expression.GreaterThan(property, constant); // >
                            break;
                            }
                        case FilterType.LessThan:
                            {
                            condition = Expression.LessThan(property, constant); // <
                            break;
                            }
                        case FilterType.GreaterThanOrEqual:
                            {
                            condition = Expression.GreaterThanOrEqual(property, constant); // >=
                            break;
                            }
                        case FilterType.LessThanOrEqual:
                            {
                            condition = Expression.LessThanOrEqual(property, constant); // <=
                            break;
                            }
                        case FilterType.NotEqual:
                            {
                            condition = Expression.NotEqual(property, constant); // !=
                            break;
                            }
                        default: // ==
                            {
                                condition = Expression.Equal(property, constant);
                                break;
                            }
                    }
                conditions.Add(condition);
            }

            if (!conditions.Any()) return x => true;

            var finalExpression = conditions.Aggregate(Expression.AndAlso);
            return Expression.Lambda<Func<T, bool>>(finalExpression, parameter);
        }

        //public static Specification<T> CreateFilterUseSpecification<T>(object filterObject)
        //{
        //    var specification = new Specification<T>();
        //    var filter = CreateFilter<T>(filterObject);
        //    specification.Criteria = filter;
        //    return specification;
        //}

    }
}