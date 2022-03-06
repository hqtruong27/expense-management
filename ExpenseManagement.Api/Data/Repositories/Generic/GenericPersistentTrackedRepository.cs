using ExpenseManagement.Api.Data.Models.Base;

namespace ExpenseManagement.Api.Data.Repositories.Generic
{
    public abstract class GenericPersistentTrackedRepository<T> : GenericRepository<T> where T : class, ITrackedEntity, IPersistentEntity
    {
        private readonly HttpContext? _httpContext;

        protected GenericPersistentTrackedRepository(ExpenseManagementDbcontext dbContext, IHttpContextAccessor httpContextAccessor) : base(dbContext)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        protected string CurrentUser => _httpContext != null && _httpContext.User.Identity != null && _httpContext.User.Identity.IsAuthenticated
                                     ? _httpContext.User.Identity.Name ?? string.Empty
                                     : "Unknown";

        public async override Task<T?> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            entity.CreatedDate = DateTime.Now;
            if (string.IsNullOrEmpty(entity.CreatedBy))
            {
                entity.CreatedBy = CurrentUser;
            }

            entity.LastUpdatedDate = DateTime.Now;
            if (string.IsNullOrEmpty(entity.LastUpdatedBy))
            {
                entity.LastUpdatedBy = CurrentUser;
            }

            entity.DeletedAt = null;
            return await base.AddAsync(entity, cancellationToken);
        }

        public async override Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            entity.LastUpdatedDate = DateTime.Now;
            if (string.IsNullOrEmpty(entity.LastUpdatedBy))
            {
                entity.LastUpdatedBy = CurrentUser;
            }

            await base.UpdateAsync(entity, cancellationToken);
        }

        public async override Task BulkUpdateAsync(IList<T> items, CancellationToken cancellationToken = default)
        {
            if (items == null || !items.Any())
                return;

            await base.BulkUpdateAsync(items.Select(x => { x.LastUpdatedDate = DateTime.Now; x.LastUpdatedBy = CurrentUser; return x; }).ToList(), cancellationToken);
        }

        public override async Task BulkInsertAsync(IList<T> items, CancellationToken cancellationToken = default)
        {
            await base.BulkInsertAsync(items.Select(x =>
            {
                x.LastUpdatedDate = DateTime.Now;
                x.LastUpdatedBy = CurrentUser;
                x.CreatedDate = DateTime.Now;
                x.CreatedBy = CurrentUser;
                return x;
            }).ToList());
        }

        public override async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                return;

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.Now;
            entity.LastUpdatedBy = CurrentUser;
            entity.LastUpdatedDate = DateTime.Now;

            await UpdateAsync(entity, cancellationToken);
        }

        public override async Task BulkDeleteAsync(IList<T> items, CancellationToken cancellationToken = default)
        {
            if (items == null || !items.Any())
                return;

            await BulkUpdateAsync(items.Select(x => { x.IsDeleted = true; x.DeletedAt = DateTime.Now; return x; }).ToList(), cancellationToken);
        }
    }
}
