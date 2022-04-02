using System.Linq.Expressions;

namespace ExpenseManagement.Api.Data.Repositories.Generic
{
    public interface IRepository<TSource>
    {
        ExpenseManagementDbcontext Context { get; }
        IQueryable<TSource> GetAll(string? include = default);
        Task<IQueryable<TSource>> FindBy(Expression<Func<TSource, bool>> predicate, string? include = default);
        IQueryable<TSource> Where(Expression<Func<TSource, bool>> predicate, string? include = default);
        Task<TSource?> FirstOrDefaultAsync(Expression<Func<TSource, bool>> expression, string? include = default, CancellationToken cancellationToken = default);
        Task<TSource?> SingleOrDefaultAsync(Expression<Func<TSource, bool>> expression, string? include = default, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default);
        Task<TSource?> AddAsync(TSource entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(TSource entity, CancellationToken cancellationToken = default);
        void Update(TSource entity);
        Task UpdateAsync(TSource entity, CancellationToken cancellationToken = default);
        Task BulkInsertAsync(IList<TSource> items, CancellationToken cancellationToken = default);
        Task BulkDeleteAsync(IList<TSource> items, CancellationToken cancellationToken = default);
        Task BulkUpdateAsync(IList<TSource> items, CancellationToken cancellationToken = default);
    }
}