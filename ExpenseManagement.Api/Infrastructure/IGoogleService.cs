namespace ExpenseManagement.Api.Infrastructure
{
    public interface IGoogleService
    {
        Task<Google.Apis.Auth.GoogleJsonWebSignature.Payload> VerifyTokenAsync(string idToken);
    }
}
