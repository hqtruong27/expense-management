using System.Linq.Expressions;

namespace ExpenseManagement.Api.Data.Repositories.Generic
{
    public interface IRepository<T>
    {
        IQueryable<T> GetAll(string? include = default);
        Task<IQueryable<T>> FindBy(Expression<Func<T, bool>> predicate, string? include = default);
        IQueryable<T> Where(Expression<Func<T, bool>> predicate, string? include = default);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> expression, string? include = default, CancellationToken cancellationToken = default);
        Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> expression, string? include = default, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<T?> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        void Update(T entity);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task BulkInsertAsync(IList<T> items, CancellationToken cancellationToken = default);
        Task BulkDeleteAsync(IList<T> items, CancellationToken cancellationToken = default);
        Task BulkUpdateAsync(IList<T> items, CancellationToken cancellationToken = default);
    }
}