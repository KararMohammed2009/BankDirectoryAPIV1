using BankDirectoryApi.Domain.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Domain.Interfaces.Specifications
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; }
        List<Expression<Func<T, object>>> Includes { get; }
        List<string>? IncludeStrings { get; }
        int? PageNumber { get; }
        int? PageSize { get; }
        bool IsPagingEnabled { get; }
        bool AsNoTracking { get; }
        List<Ordering<T>> Orderings { get; }
    }
}
