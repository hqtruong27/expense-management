using ExpenseManagement.Api.Common;
using ExpenseManagement.Api.Enum;
using System.ComponentModel.DataAnnotations;

namespace ExpenseManagement.Api.Model
{
    public class ExpenseIndexRequest : QueryStringParameters
    {
        public ExpenseStatus? Status { get; set; }
    }

    public class ExpenseCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ExpenseType? Type { get; set; }
        public decimal Amount { get; set; }
    }
    public class ExpenseUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}