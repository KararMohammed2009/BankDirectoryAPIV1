
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Domain.Classes.Pagination
{
    /// <summary>
    /// Represents a paginated response containing a list of items and pagination information.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginatedResponse<T>
    {
        public List<T> Items { get; set; }
        public PaginationInfo Pagination { get; set; }
        public int TotalItems { get; set; }
        public bool HasNextPage => Pagination?.PageNumber < TotalItems / Pagination?.PageSize;
        public bool HasPreviousPage => (Pagination?.PageNumber > 1);
        /// <summary>
        /// Converts the current paginated response to a new type of paginated response.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="newItems"></param>
        /// <returns>The new paginated response with the new items.</returns>
        public PaginatedResponse<U> To<U>(List<U> newItems)
        {
            return new PaginatedResponse<U>
            {
                Items = newItems,
                Pagination = this.Pagination,  
                TotalItems = this.TotalItems    
                
            };
        }
    }
}
