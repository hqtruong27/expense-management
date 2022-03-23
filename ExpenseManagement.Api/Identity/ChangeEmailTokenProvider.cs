using ExpenseManagement.Api.Data.Models;
using ExpenseManagement.Api.Enum;
using ExpenseManagement.Api.Middleware;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace ExpenseManagement.Api.Identity
{
    public class ChangeEmailTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
    {
        public ChangeEmailTokenProvider(IDataProtectionProvider dataProtectionProvider,
            IOptions<DataProtectionTokenProviderOptions> options,
            ILogger<DataProtectorTokenProvider<TUser>> logger)
            : base(dataProtectionProvider, options, logger)
        {

        }
        //public override async Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user)
        //{
        //    var token = await manager.CreateSecurityTokenAsync(user).ConfigureAwait(false);
        //    var currentTimeStep = Rfc6238AuthenticationService.GetCurrentTimeStepNumber();
        //    return Rfc6238AuthenticationService.ComputeTotp(token, currentTimeStep, string.Empty)
        //        .ToString("D6", System.Globalization.CultureInfo.InvariantCulture);
        //}

        //public override async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user)
        //{
        //    int code;
        //    if (!int.TryParse(token, out code))
        //    {
        //        return false;
        //    }
        //    var securityToken = await manager.CreateSecurityTokenAsync(user).ConfigureAwait(false);

        //    return securityToken != null && Rfc6238AuthenticationService.ValidateCode(securityToken, code, string.Empty);
        //}

        public class ChangeEmaiTokenProviderOptions : DataProtectionTokenProviderOptions
        {
            public ChangeEmaiTokenProviderOptions()
            {
                Name = "Email2";
                TokenLifespan = TimeSpan.FromSeconds(10);
            }
        }
    }

    public class ChangeEmailTotpSecurityStampTokenProvider<TUser> : TotpSecurityStampBasedTokenProvider<TUser> where TUser : class
    {
        private readonly ITotpSercurityTokenRepository _totpSercurityToken;
        private readonly TotpSercurityTokenType _changeEmail = TotpSercurityTokenType.ChangeEmail;
        private const int _min = 100000;
        private const int _max = 999999;
        public ChangeEmailTotpSecurityStampTokenProvider(ITotpSercurityTokenRepository totpSercurityToken)
        {
            _totpSercurityToken = totpSercurityToken;
        }

        public override async Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user)
        {
            var email = await manager.GetEmailAsync(user).ConfigureAwait(false);

            return !string.IsNullOrWhiteSpace(email) && await manager.IsEmailConfirmedAsync(user).ConfigureAwait(false);
        }

        public override async Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user)
        {
            string? code = default;
            var any = true;
            while (any || code == default)
            {
                //generate 6 digit random number
                code = RandomNumberGenerator.GetInt32(_min, _max).ToString();
                any = await _totpSercurityToken.AnyAsync(x => x.Code == code.ToString() && x.Email == purpose && x.Type == _changeEmail);
            }

            //var email = await manager.GetEmailAsync(user);
            await _totpSercurityToken.AddAsync(new TotpSercurityToken
            {
                Code = code,
                Email = purpose,
                Expired = DateTimeOffset.Now.AddMinutes(30),
                Type = Enum.TotpSercurityTokenType.ChangeEmail,
            });

            return code;
        }

        //public override async Task<string> GetUserModifierAsync(string purpose, UserManager<TUser> manager, TUser user)
        //{
        //    var email = await manager.GetEmailAsync(user);
        //    return $"ChangeEmail:{purpose}:{email}";
        //}

        public override async Task<bool> ValidateAsync(string purpose, string code, UserManager<TUser> manager, TUser user)
        {
            var topt = await _totpSercurityToken.Where(x => x.Code == code && x.Email == purpose && x.Type == _changeEmail)
                                                .OrderByDescending(x => x.Expired)
                                                .FirstOrDefaultAsync();
            if (topt == null)
            {
                return false;
            }
            else
            if (topt.IsUsed)
            {
                throw new BadRequestException(Messages.OTPUsed);
            }
            else
            if (topt.Expired < DateTimeOffset.Now)
            {
                throw new BadRequestException(Messages.OTPExpired);
            }

            return true;
        }
    }
}