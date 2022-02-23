using System.ComponentModel.DataAnnotations;

namespace ExpenseManagement.Api.Enum
{
    public enum Provider
    {
        [Display(Name = "Google")]
        Google,
        [Display(Name = "Facebook")]
        Facebook,
        [Display(Name = "Microsoft")]
        Microsoft,
        [Display(Name = "Twitter")]
        Twitter
    }
}
