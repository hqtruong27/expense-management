using Microsoft.AspNetCore.Identity;

namespace ExpenseManagement.Api.Identity
{
    public class ExpenseTokenOptions : TokenOptions
    {
        public const string ChangeEmailProvider = "ChangeEmail";
    }
}
