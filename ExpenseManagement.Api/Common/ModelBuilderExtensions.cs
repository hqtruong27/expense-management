using ExpenseManagement.Api.Data.Models;
using ExpenseManagement.Api.Data.Models.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpenseManagement.Api.Common
{
    public static class ModelBuilderExtensions
    {
        private static readonly MethodInfo _propertyMethod = typeof(EF).GetMethods(BindingFlags.Static | BindingFlags.Public)
                                                                       .First(x => x.Name == nameof(EF.Property))
                                                                       .MakeGenericMethod(typeof(bool));
        public static void FilterDeletedRecords(this ModelBuilder modelBuilder)
        {
            foreach (var entity in from entity in modelBuilder.Model.GetEntityTypes()
                                   where typeof(IPersistentEntity).IsAssignableFrom(entity.ClrType)
                                   && !typeof(Role).IsAssignableFrom(entity.ClrType) //Exclude Role type
                                   select entity)
            {
                modelBuilder.Entity(entity.ClrType).HasQueryFilter(GetIsDeletedRestriction(entity.ClrType));
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