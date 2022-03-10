using AutoMapper;
using ExpenseManagement.Api.Data.Models;
using ExpenseManagement.Api.Model;

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
}
