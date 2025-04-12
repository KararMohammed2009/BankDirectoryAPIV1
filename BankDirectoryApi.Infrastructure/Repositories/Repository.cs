using BankDirectoryApi.Domain.Classes.Pagination;
using BankDirectoryApi.Domain.Interfaces;
using BankDirectoryApi.Domain.Interfaces.Specifications;
using BankDirectoryApi.Infrastructure.Data;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;
using System.Net;

namespace BankDirectoryApi.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class 
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

          public async Task<Result<IEnumerable<T>>> GetAllAsync(CancellationToken cancellationToken) => 
            await _dbSet.ToListAsync(cancellationToken);

       
        public async Task<Result<PaginatedResponse<T>>>
           GetAllAsync(ISpecification<T> spec, CancellationToken cancellationToken)
        {
            var response = new PaginatedResponse<T>();
            var query = _dbSet.AsQueryable();

            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }

            if (spec.Includes != null)
            {
                query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
            }

            if (spec.IncludeStrings != null)
            {
                foreach (var include in spec.IncludeStrings)
                {
                    query = query.Include(include);
                }
            }

            if (spec.Orderings != null && spec.Orderings.Any())
            {
                IOrderedQueryable<T>? orderedQuery = null;

                for (int i = 0; i < spec.Orderings.Count; i++)
                {
                    var ordering = spec.Orderings[i];
                    if (i == 0)
                    {
                        if (ordering.IsDescending)
                        {
                            orderedQuery = query.OrderByDescending(ordering.OrderBy);
                        }
                        else
                        {
                            orderedQuery = query.OrderBy(ordering.OrderBy);
                        }
                    }
                    else if(orderedQuery != null)
                    {
                        if (ordering.IsDescending)
                        {
                            orderedQuery = orderedQuery.ThenByDescending(ordering.OrderBy);
                        }
                        else
                        {
                            orderedQuery = orderedQuery.ThenBy(ordering.OrderBy);
                        }
                    }
                }
                query = orderedQuery ?? query;
            }

            if (spec.IsPagingEnabled && spec.PageNumber.HasValue && spec.PageSize.HasValue)
            {
                query = query.Skip((spec.PageNumber.Value - 1) * spec.PageSize.Value)
                    .Take(spec.PageSize.Value);
                response.Pagination = new PaginationInfo
                {
                    PageNumber = spec.PageNumber.Value,
                    PageSize = spec.PageSize.Value
                };
            }

            if (spec.AsNoTracking)
            {
                query = query.AsNoTracking();
            }
            response.TotalItems = await query.CountAsync(cancellationToken);
            response.Items = await query.ToListAsync(cancellationToken);
            return response;
        }



        public async Task<Result<T>> GetByIdAsync(int id)  {
            var result = await _dbSet.FindAsync(id);
            if (result == null)
            {
                return Result.Fail(new Error($"Entity with id {id} not found").WithMetadata
                    ("StatusCode",HttpStatusCode.NotFound));
            }
            return Result.Ok(result);
        }

      

        public async Task<Result<IEnumerable<T>>> FindAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.Where(predicate).ToListAsync();

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            
        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
