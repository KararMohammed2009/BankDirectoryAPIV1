

using BankDirectoryApi.Common.Interfaces;

namespace BankDirectoryApi.Common.Extensions
{
    public static class PaginateExtension
    {
        public static IQueryable<T> Paginate<T>(
          this IQueryable<T> source,
          IPaginationInfo pagination)
        {
            return source
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize);
        }
    }
}
