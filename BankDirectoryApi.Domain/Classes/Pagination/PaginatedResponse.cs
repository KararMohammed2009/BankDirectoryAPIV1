
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Domain.Classes.Pagination
{
    public class PaginatedResponse<T>
    {
        public List<T> Items { get; set; }
        public PaginationInfo Pagination { get; set; }
        public int TotalItems { get; set; }
        public bool HasNextPage => Pagination?.PageNumber < TotalItems / Pagination?.PageSize;
        public bool HasPreviousPage => (Pagination?.PageNumber > 1);
    }
}
