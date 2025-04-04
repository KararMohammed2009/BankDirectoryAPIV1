using BankDirectoryApi.Domain.Classes.Pagination;
using BankDirectoryApi.Domain.Interfaces.Specifications;
using FluentResults;
using System.Linq.Expressions;

namespace BankDirectoryApi.Domain.Interfaces
{
    public interface IRepository<T> where T : class
   
    {
        Task<Result<T>> GetByIdAsync(int id);
        Task<Result<IEnumerable<T>>> GetAllAsync(CancellationToken cancellationToken);
        Task<Result<PaginatedResponse<T>>> GetAllAsync(ISpecification<T> specification,CancellationToken cancellationToken);
        Task<Result<IEnumerable<T>>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task SaveChangesAsync();
    }
}
