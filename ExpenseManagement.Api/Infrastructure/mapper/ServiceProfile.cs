using AutoMapper;
using ExpenseManagement.Api.Data.Models;
using ExpenseManagement.Api.Model;
using System.Linq.Expressions;

namespace ExpenseManagement.Api.Infrastructure.mapper
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            CreateMap<CreateUserRequest, User>();
            CreateMap<UpdateUserRequest, User>();
            CreateMap<User, UserResponse>();
            CreateMap<Expense, ExpenseResponse>();
            CreateMap<ExpenseCreateRequest, Expense>();
            CreateMap<ExpenseUpdateRequest, Expense>();
            CreateMap<DebtCharge, DebtChargeResponse>();
            CreateMap<DebtReminderCreateRequest, DebtReminder>();
            CreateMap<UserExpense, UserExpenseResponse>();
        }
    }

    public static class MappingExtensions
    {
        public static IMappingExpression<TSource, TDestination> MapFrom<TSource, TDestination, TMember, TSourceMember>(
            this IMappingExpression<TSource, TDestination> mapping,
            Expression<Func<TDestination, TMember>> destinationMember,
            Expression<Func<TSource, TSourceMember>> sourceMember)
            => mapping.ForMember(destinationMember, x => x.MapFrom(sourceMember));
    }

}
