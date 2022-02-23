using ExpenseManagement.Api.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpenseManagement.Api.Data
{
    public class ExpenseManagementDbcontext : IdentityDbContext<User, Role, string>
    {
        //private static readonly MethodInfo? _propertyMethod = typeof(EF).GetMethod(nameof(EF.Property), BindingFlags.Static | BindingFlags.Public)?.MakeGenericMethod(typeof(bool));

        public ExpenseManagementDbcontext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Expense>? Expenses { get; set; }
        public DbSet<UserExpense>? UserExpenses { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserExpense>()
                   .HasKey(p => new { p.UserId, p.ExpenseId });

            base.OnModelCreating(builder);
        }
    }
    //private static LambdaExpression GetIsDeletedRestriction(Type type)
    //{
    //    var param = Expression.Parameter(type, "it");
    //    var prop = Expression.Call(_propertyMethod  , param, Expression.Constant("IsDeleted"));
    //    var condition = Expression.MakeBinary(ExpressionType.Equal, prop, Expression.Constant(false));
    //    var lambda = Expression.Lambda(condition, param);
    //    return lambda;
    //}
}
