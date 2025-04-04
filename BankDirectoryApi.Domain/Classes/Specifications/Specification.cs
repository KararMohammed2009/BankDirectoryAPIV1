using BankDirectoryApi.Domain.Interfaces.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Domain.Classes.Specifications
{
    public class Specification<T> : ISpecification<T>
    {
       

        public Expression<Func<T, bool>> Criteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; }
        public List<string> IncludeStrings { get; set; }
        public int? PageNumber { get; set; } 
        public int? PageSize { get; set; }
        public bool IsPagingEnabled { get; set; } = false;
        public bool AsNoTracking { get; set; } = false;
        public List<Ordering<T>> Orderings { get; set; }


       

    }
}
