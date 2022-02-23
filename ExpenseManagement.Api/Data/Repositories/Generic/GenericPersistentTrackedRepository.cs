using ExpenseManagement.Api.Data.Models.Base;

namespace ExpenseManagement.Api.Data.Repositories.Generic
{
    public abstract class GenericPersistentTrackedRepository<T> : GenericRepository<T> where T : class, ITrackedEntity, IPersistentEntity
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected GenericPersistentTrackedRepository(ExpenseManagementDbcontext dbContext, IHttpContextAccessor httpContextAccessor) : base(dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected string CurrentUser => _httpContextAccessor.HttpContext?.User.Identity != null && _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated
                ? _httpContextAccessor.HttpContext.User.Identity.Name ?? string.Empty
                : "Unknown";

        public async override Task<T?> AddAsync(T entity)
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
            return await base.AddAsync(entity);
        }

        public async override Task UpdateAsync(T entity)
        {
            entity.LastUpdatedDate = DateTime.Now;
            entity.LastUpdatedBy = CurrentUser;
            await base.UpdateAsync(entity);
        }

        public async override Task BulkUpdateAsync(IList<T> items)
        {
            if (items == null || !items.Any())
                return;

            await base.BulkUpdateAsync(items.Select(x => { x.LastUpdatedDate = DateTime.Now; x.LastUpdatedBy = CurrentUser; return x; }).ToList());
        }

        public override async Task BulkInsertAsync(IList<T> items)
        {
            await base.BulkInsertAsync(items.Select(x => { x.LastUpdatedDate = DateTime.Now; x.LastUpdatedBy = CurrentUser; x.CreatedDate = DateTime.Now; x.CreatedBy = CurrentUser; return x; }).ToArray());
        }

        public override async Task DeleteAsync(T entity)
        {
            if (entity == null)
                return;

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.Now;
            entity.LastUpdatedBy = CurrentUser;
            entity.LastUpdatedDate = DateTime.Now;

            await UpdateAsync(entity);
        }

        public override async Task BulkDeleteAsync(IList<T> items)
        {
            if (items == null || !items.Any())
                return;

            await BulkUpdateAsync(items.Select(x =>
            {
                x.IsDeleted = true; x.DeletedAt = DateTime.Now; return x;
            }).ToList());
        }
    }
}
