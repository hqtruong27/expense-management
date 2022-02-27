using AutoMapper;
using ExpenseManagement.Api.Data.Models;
using ExpenseManagement.Api.Model;

namespace ExpenseManagement.Api.mapper
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
        }
    }
}
