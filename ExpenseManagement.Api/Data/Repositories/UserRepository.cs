using ExpenseManagement.Api.Data;
using ExpenseManagement.Api.Data.Models;
using ExpenseManagement.Api.Data.Repositories.Generic;
using ExpenseManagement.Api.Enum;
using ExpenseManagement.Api.Infrastructure;
using ExpenseManagement.Api.Model;
using ExpenseManagement.Api.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ExpenseManagement.Api.Data.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<JwtTokenResponse> AuthenticateAsync(string username, string password);
        Task<IdentityResult> CreateAsync(User user);
        Task<JwtTokenResponse> ExternalLoginAsync(ExternalLoginRequest request);
        Task<User> FindByEmailAsync(string email);
        Task<string> GeneratePasswordResetTokenAsync(User user);
        Task<string> GetTokenAsync(User user);
        Task<IdentityResult> ResetPasswordAsync(User user, string code, string password);
    }
    public class UserRepository : GenericPersistentTrackedRepository<User>, IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IFacebookService _facebookService;
        private readonly IGoogleService _googleService;
        private readonly Authentication _authentication;
        private readonly ILogger<UserRepository> _logger;
        public UserRepository(ExpenseManagementDbcontext dbContext, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, Authentication authentication, SignInManager<User> signInManager, ILogger<UserRepository> logger, IFacebookService facebookService, IGoogleService googleService) : base(dbContext, httpContextAccessor)
        {
            _userManager = userManager;
            _authentication = authentication;
            _signInManager = signInManager;
            _logger = logger;
            _facebookService = facebookService;
            _googleService = googleService;
        }

        public async Task<JwtTokenResponse> AuthenticateAsync(string username, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                var user = await _userManager.FindByNameAsync(username);
                var (token, validTo) = await GenerateJwtTokenAsync(user);
                return new JwtTokenResponse
                {
                    IsAuthenticated = true,
                    FullName = $"{user.Surname} {user.GivenName}",
                    Token = token,
                    Email = user.Email,
                    Avatar = user.Avatar,
                    ValidTo = validTo,
                };
            }
            if (result.RequiresTwoFactor)
            {
                _logger.LogWarning("User required 2fa.");
                return new JwtTokenResponse { Description = "User required 2fa." };
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return new JwtTokenResponse { Description = "User account locked out." };
            }
            else
            {
                return new JwtTokenResponse { Description = "Invalid login attempt." };
            }
        }

        public Task<IdentityResult> CreateAsync(User user)
        {
            return _userManager.CreateAsync(user);
        }

        public async Task<JwtTokenResponse> ExternalLoginAsync(ExternalLoginRequest request)
        {
            _logger.LogInformation("START: exteral login {request}", request);
            var token = request.Token;
            var providerKey = string.Empty;
            var provider = request.Provider;
            var email = string.Empty;
            var surname = string.Empty;
            var givenName = string.Empty;
            var emailVerified = true;
            var picture = string.Empty;
            switch (provider)
            {
                case Provider.Google:
                    var payload = await _googleService.VerifyTokenAsync(token);
                    if (payload != null)
                    {
                        providerKey = payload.Subject;
                        email = payload.Email;
                        surname = payload.FamilyName;
                        givenName = payload.GivenName;
                        picture = payload.Picture;
                        emailVerified = payload.EmailVerified;
                        break;
                    }

                    return new JwtTokenResponse { Description = "Token invalid." };
                case Provider.Facebook:
                    var facebook = await _facebookService.VerifyTokenAsync(token);
                    if (facebook != null && facebook.Data.IsValid)
                    {
                        var userInfo = await _facebookService.GetUserInfoAsync(token);
                        if (userInfo != null)
                        {
                            providerKey = userInfo.Id;
                            email = userInfo.Email;
                            surname = userInfo.Surname;
                            givenName = userInfo.GivenName;
                            picture = userInfo.Picture.Data.Url.ToString();
                        }

                        break;
                    }

                    return new JwtTokenResponse { Description = "Token invalid." };
                case Provider.Microsoft:
                    return new JwtTokenResponse { Description = "Invalid External Authentication." };
                case Provider.Twitter:
                    return new JwtTokenResponse { Description = "Invalid External Authentication." };
                default:
                    return new JwtTokenResponse { Description = "Invalid External Authentication." };
            }

            var info = new UserLoginInfo(provider.ToString(), providerKey, provider.ToString());
            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            var result = await _signInManager.ExternalLoginSignInAsync(provider.ToString(), providerKey, false);
            if (result.Succeeded)
                if (user == null)
                {
                    user = await _userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        user = new User
                        {
                            Email = email,
                            UserName = email,
                            Avatar = picture,
                            Surname = surname,
                            GivenName = givenName,
                            EmailConfirmed = emailVerified,
                        };

                        await _userManager.CreateAsync(user);
                        //prepare and send an email for the email confirmation
                        //await _userManager.AddToRoleAsync(user, "Viewer");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(user.Email))
                        {
                            user.Email = email;
                        }

                        if (string.IsNullOrWhiteSpace($"{user.Surname} {user.GivenName}"))
                        {
                            user.Surname = surname;
                            user.GivenName = givenName;
                        }

                        if (string.IsNullOrWhiteSpace(user.Avatar))
                        {
                            user.Avatar = picture;
                        }

                        await _userManager.UpdateAsync(user);
                    }

                    await _userManager.AddLoginAsync(user, info);

                    //TODO: check for the Locked out account
                    //logic code here...

                    var (accessToken, validTo) = await GenerateJwtTokenAsync(user);

                    _logger.LogInformation("END: success exteral login {provider}", provider);
                    return new JwtTokenResponse
                    {
                        IsAuthenticated = true,
                        FullName = $"{user.Surname} {user.GivenName}",
                        Token = accessToken,
                        Email = user.Email,
                        Avatar = user.Avatar,
                        ValidTo = validTo,
                    };
                }
            if (result.RequiresTwoFactor)
            {
                _logger.LogWarning("User required 2fa.");
                return new JwtTokenResponse { Description = "User required 2fa." };
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return new JwtTokenResponse { Description = "User account locked out." };
            }
            else
            {
                return new JwtTokenResponse { Description = "Invalid login attempt." };
            }
        }

        public Task<string> GetTokenAsync(User user)
        {
            throw new NotImplementedException();
        }

        private async Task<(string token, string validTo)> GenerateJwtTokenAsync(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = System.Text.Encoding.ASCII.GetBytes(_authentication.JWT.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Surname, user.Surname ?? string.Empty),
                    new Claim(ClaimTypes.GivenName, user.GivenName ?? string.Empty),
                }),
                TokenType = "Bearer",
                Issuer = _authentication.JWT.ValidIssuer,
                Audience = _authentication.JWT.ValidAudience,
                Expires = DateTime.UtcNow.AddDays(_authentication.JWT.Expired),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            user.LastLogin = DateTime.Now;
            await _userManager.UpdateAsync(user);

            return (token, securityToken.ValidTo.ToString("o"));
        }

        public Task<IdentityResult> ResetPasswordAsync(User user, string code, string password)
        {
            return _userManager.ResetPasswordAsync(user, code, password);
        }

        public Task<User> FindByEmailAsync(string email)
        {
            return _userManager.FindByEmailAsync(email);
        }

        public Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return _userManager.GeneratePasswordResetTokenAsync(user);
        }
    }
}