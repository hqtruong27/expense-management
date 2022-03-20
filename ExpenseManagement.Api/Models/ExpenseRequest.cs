using ExpenseManagement.Api.Common;
using ExpenseManagement.Api.Enum;

namespace ExpenseManagement.Api.Models
{
    public class ExpenseIndexRequest : QueryStringParameters
    {
        public string SortPrice { get; set; } = string.Empty;
        public ExpenseStatus? Status { get; set; }
        public ExpenseType? Type { get; set; }
    }

    public class ExpenseCreateRequest : ExpenseUpdateRequest
    {
        public ExpenseType? Type { get; set; }
    }

    public class ExpenseUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}