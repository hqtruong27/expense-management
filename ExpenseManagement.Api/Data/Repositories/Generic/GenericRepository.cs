using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace ExpenseManagement.Api.Data.Repositories.Generic
{
    public abstract class GenericRepository<TSource> : IRepository<TSource> where TSource : class
    {
        private readonly ExpenseManagementDbcontext _context;

        public ExpenseManagementDbcontext Context => _context;

        public GenericRepository(ExpenseManagementDbcontext dbContext)
        {
            _context = dbContext;
        }

        public virtual Task<bool> AnyAsync(Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return _context.Set<TSource>().AnyAsync(predicate, cancellationToken);
        }

        public async virtual Task<TSource?> AddAsync(TSource entity, CancellationToken cancellationToken = default)
        {
            var result = _context.Set<TSource>().Add(entity);

            var insertedRows = await _context.SaveChangesAsync(cancellationToken);

            return insertedRows > 0 ? result.Entity : null;
        }

        public async virtual Task BulkInsertAsync(IList<TSource> items, CancellationToken cancellationToken = default)
        {
            await _context.BulkInsertAsync(items, cancellationToken: cancellationToken);
        }

        public void Update(TSource entity) => _context.Update(entity);

        public async virtual Task UpdateAsync(TSource entity, CancellationToken cancellationToken = default)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _context.Set<TSource>().Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async virtual Task BulkUpdateAsync(IList<TSource> items, CancellationToken cancellationToken = default)
        {
            await _context.BulkUpdateAsync(items, cancellationToken: cancellationToken);
        }

        public async virtual Task DeleteAsync(TSource entity, CancellationToken cancellationToken = default)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async virtual Task BulkDeleteAsync(IList<TSource> items, CancellationToken cancellationToken = default)
        {
            if (items == null || !items.Any())
                return;
            await _context.BulkDeleteAsync(items, cancellationToken: cancellationToken);
        }

        public async virtual Task<TSource?> FirstOrDefaultAsync(Expression<Func<TSource, bool>> predicate, string? include = default, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<TSource>().AsNoTracking();
            query = BuildIncludeQuery(query, include);
            return await query.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async virtual Task<TSource?> SingleOrDefaultAsync(Expression<Func<TSource, bool>> predicate, string? include = default, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<TSource>().AsNoTracking();
            query = BuildIncludeQuery(query, include);
            return await query.SingleOrDefaultAsync(predicate, cancellationToken);
        }

        public virtual IQueryable<TSource> GetAll(string? include = default)
        {
            var query = _context.Set<TSource>().AsNoTracking();
            query = BuildIncludeQuery(query, include);
            return query;
        }

        public async virtual Task<IQueryable<TSource>> FindBy(Expression<Func<TSource, bool>> predicate, string? include = default)
        {
            var query = _context.Set<TSource>().Where(predicate).AsNoTracking();
            query = BuildIncludeQuery(query, include);

            return await Task.FromResult(query);
        }

        public virtual IQueryable<TSource> Where(Expression<Func<TSource, bool>> predicate, string? include = default)
        {
            var query = _context.Set<TSource>().Where(predicate).AsNoTracking();
            query = BuildIncludeQuery(query, include);

            return query;
        }

        public virtual IQueryable<TSource> FromSqlRaw([NotParameterized] string sql, params object[] parameters)
        {
            return _context.Set<TSource>().FromSqlRaw(sql, parameters);
        }

        public virtual IQueryable<TSource> FromSqlInterpolated([NotParameterized] FormattableString sql)
        {
            return _context.Set<TSource>().FromSqlInterpolated(sql);
        }

        public IQueryable<TSource> BuildIncludeQuery(IQueryable<TSource> query, string? include)
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