namespace ExpenseManagement.Api.Infrastructure
{
    public interface IFacebookService
    {
        Task<FacebookInfoResponse?> VerifyTokenAsync(string accessToken);
        Task<FacebookUserInfoResponse?> GetUserInfoAsync(string accessToken);
    }
}
