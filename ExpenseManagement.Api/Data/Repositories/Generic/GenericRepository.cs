using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace ExpenseManagement.Api.Data.Repositories.Generic
{
    public abstract class GenericRepository<T> : IRepository<T> where T : class
    {
        private readonly ExpenseManagementDbcontext _context;

        public ExpenseManagementDbcontext Context => _context;

        public GenericRepository(ExpenseManagementDbcontext dbContext)
        {
            _context = dbContext;
        }

        public virtual Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return _context.Set<T>().AnyAsync(predicate, cancellationToken);
        }

        public async virtual Task<T?> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            var result = _context.Set<T>().Add(entity);

            var insertedRows = await _context.SaveChangesAsync(cancellationToken);

            return insertedRows > 0 ? result.Entity : null;
        }

        public async virtual Task BulkInsertAsync(IList<T> items, CancellationToken cancellationToken = default)
        {
            await _context.BulkInsertAsync(items, cancellationToken: cancellationToken);
        }

        public void Update(T entity) => _context.Update(entity);

        public async virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _context.Set<T>().Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async virtual Task BulkUpdateAsync(IList<T> items, CancellationToken cancellationToken = default)
        {
            await _context.BulkUpdateAsync(items, cancellationToken: cancellationToken);
        }

        public async virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async virtual Task BulkDeleteAsync(IList<T> items, CancellationToken cancellationToken = default)
        {
            if (items == null || !items.Any())
                return;
            await _context.BulkDeleteAsync(items, cancellationToken: cancellationToken);
        }

        public async virtual Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, string? include = default, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<T>().AsNoTracking();
            query = BuildIncludeQuery(query, include);
            return await query.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async virtual Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, string? include = default, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<T>().AsNoTracking();
            query = BuildIncludeQuery(query, include);
            return await query.SingleOrDefaultAsync(predicate, cancellationToken);
        }

        public virtual IQueryable<T> GetAll(string? include = default)
        {
            var query = _context.Set<T>().AsNoTracking();
            query = BuildIncludeQuery(query, include);
            return query;
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

        public virtual IQueryable<T> FromSqlRaw([NotParameterized] string sql, params object[] parameters)
        {
            return _context.Set<T>().FromSqlRaw(sql, parameters);
        }

        public virtual IQueryable<T> FromSqlInterpolated([NotParameterized] FormattableString sql)
        {
            return _context.Set<T>().FromSqlInterpolated(sql);
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