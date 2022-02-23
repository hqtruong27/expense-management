using EFCore.BulkExtensions;
using ExpenseManagement.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExpenseManagement.Api.Data.Repositories.Generic
{
    public interface IRepository<T>
    {
        Task<IQueryable<T>> GetAll(string? include = default);
        Task<IQueryable<T>> FindBy(Expression<Func<T, bool>> predicate, string? include = default);
        IQueryable<T> Where(Expression<Func<T, bool>> predicate, string? include = default);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> expression, string? include = default);
        Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> expression, string? include = default);
        Task<T?> AddAsync(T entity);
        Task DeleteAsync(T entity);
        void Update(T entity);
        Task UpdateAsync(T entity);
        Task BulkInsertAsync(IList<T> items);
        Task BulkDeleteAsync(IList<T> items);
        Task BulkUpdateAsync(IList<T> items);
    }

    public abstract class GenericRepository<T> : IRepository<T> where T : class
    {
        private readonly ExpenseManagementDbcontext _context;

        public GenericRepository(ExpenseManagementDbcontext dbContext)
        {
            _context = dbContext;
        }

        public async virtual Task<T?> AddAsync(T entity)
        {
            var result = _context.Set<T>().Add(entity);

            var insertedRows = await _context.SaveChangesAsync();

            return insertedRows > 0 ? result.Entity : null;
        }

        public async virtual Task BulkInsertAsync(IList<T> items)
        {
            await _context.BulkInsertAsync(items);
        }

        public void Update(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _context.Set<T>().Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
        }

        public async virtual Task UpdateAsync(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _context.Set<T>().Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
        }

        public async virtual Task BulkUpdateAsync(IList<T> items)
        {
            await _context.BulkUpdateAsync(items);
        }

        public async virtual Task DeleteAsync(T entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async virtual Task BulkDeleteAsync(IList<T> items)
        {
            if (items == null || !items.Any())
                return;
            await _context.BulkDeleteAsync(items);
        }

        public async virtual Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, string? include = default)
        {
            var query = _context.Set<T>().AsNoTracking();
            query = BuildIncludeQuery(query, include);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async virtual Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, string? include = default)
        {
            var query = _context.Set<T>().AsNoTracking();
            query = BuildIncludeQuery(query, include);
            return await query.SingleOrDefaultAsync(predicate);
        }

        public async virtual Task<IQueryable<T>> GetAll(string? include = default)
        {
            var query = _context.Set<T>().AsNoTracking();
            query = BuildIncludeQuery(query, include);
            return await Task.FromResult(query);
        }

        public async virtual Task<IQueryable<T>> FindBy(Expression<Func<T, bool>> predicate, string? include = default)
        {
            var query = _context.Set<T>().Where(predicate).AsNoTracking();
            query = BuildIncludeQuery(query, include);

            return await Task.FromResult(query);
        }

        public virtual IQueryable<T> Where(Expression<Func<T, bool>> predicate, string? include = default)
        {
            var query = _context.Set<T>().Where(predicate).AsNoTracking();
            query = BuildIncludeQuery(query, include);

            return query;
        }

        public IQueryable<T> BuildIncludeQuery(IQueryable<T> query, string? include)
        {
            if (string.IsNullOrWhiteSpace(include))
            {
                return query;
            }

            var includeEntities = include.Split(',');
            if (includeEntities == null || !includeEntities.Any())
                return query;

            foreach (var entity in includeEntities)
            {
                query = query.Include(entity);
            }

            return query;
        }
    }
}