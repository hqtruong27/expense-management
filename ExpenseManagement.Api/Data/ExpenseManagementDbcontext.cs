using ExpenseManagement.Api.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpenseManagement.Api.Data
{
    public class ExpenseManagementDbcontext : IdentityDbContext<User, Role, string>
    {
        private static readonly MethodInfo _propertyMethod = typeof(EF).GetMethods(BindingFlags.Static | BindingFlags.Public)
                                                                       .First(x => x.Name == nameof(EF.Property)).MakeGenericMethod(typeof(bool));

        public ExpenseManagementDbcontext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Expense>? Expenses { get; set; }
        public DbSet<UserExpense>? UserExpenses { get; set; }
        public DbSet<DebtCharge>? DebtCharges { get; set; }
        public DbSet<TransactionHistory>? TransactionHistories { get; set; }
        public DbSet<Log>? Logs { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserExpense>()
                   .HasKey(p => new { p.UserId, p.ExpenseId });

            builder.Entity<DebtCharge>()
                .HasOne(s => s.Creditor)
                .WithMany(u => u.Creditors)
                .HasForeignKey(u => u.CreditorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DebtCharge>()
                .HasOne(s => s.Debtor)
                .WithMany(u => u.Debtors)
                .HasForeignKey(u => u.DebtorId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(builder);
            FilterDeletedRecords(builder);
        }

        private void FilterDeletedRecords(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(Models.Base.IPersistentEntity).IsAssignableFrom(entity.ClrType) && !typeof(User).IsAssignableFrom(entity.ClrType) && !typeof(Role).IsAssignableFrom(entity.ClrType)) //Exclude User type
                {
                    modelBuilder.Entity(entity.ClrType).HasQueryFilter(GetIsDeletedRestriction(entity.ClrType));
                }
            }
        }

        private static LambdaExpression GetIsDeletedRestriction(Type type)
        {
            var param = Expression.Parameter(type, "it");
            var prop = Expression.Call(_propertyMethod, param, Expression.Constant("IsDeleted"));
            var condition = Expression.MakeBinary(ExpressionType.Equal, prop, Expression.Constant(false));
            var lambda = Expression.Lambda(condition, param);
            return lambda;
        }
    }
}
