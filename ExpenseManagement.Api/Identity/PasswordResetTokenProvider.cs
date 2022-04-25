using ExpenseManagement.Api.Middleware;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace ExpenseManagement.Api.Identity
{
    public class PasswordResetTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
    {
        public PasswordResetTokenProvider(IDataProtectionProvider dataProtectionProvider,
            IOptions<DataProtectionTokenProviderOptions> options,
            ILogger<DataProtectorTokenProvider<TUser>> logger)
            : base(dataProtectionProvider, options, logger)
        {
        }

        public class PasswordResetTokenProviderOptions : DataProtectionTokenProviderOptions
        {
            public PasswordResetTokenProviderOptions()
            {
                Name = "PasswordResetTokenProvider";
                TokenLifespan = TimeSpan.FromDays(1);
            }
        }
    }

    public class PasswordResetTotpSecurityStampTokenProvider<TUser> : TotpSecurityStampBasedTokenProvider<TUser> where TUser : class
    {
        //Clear code later
        private readonly ITotpSercurityTokenRepository _totpSercurityToken;
        private readonly TotpSercurityTokenType _passwordReset = TotpSercurityTokenType.PasswordReset;
        private const int _min = 100000;
        private const int _max = 999999;

        public PasswordResetTotpSecurityStampTokenProvider(ITotpSercurityTokenRepository totpSercurityToken)
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
                code = RandomNumberGenerator.GetInt32(_min, _max).ToString();
                any = await _totpSercurityToken.AnyAsync(x => x.Code == code.ToString() && x.Email == purpose && x.Type == _passwordReset);
            }
            
            await _totpSercurityToken.AddAsync(new Data.Models.TotpSercurityToken
            {
                Code = code,
                Email = purpose,
                Expired = DateTimeOffset.Now.AddMinutes(30),
                Type = TotpSercurityTokenType.PasswordReset,
            });

            return code;
        }
        
        public override async Task<bool> ValidateAsync(string purpose, string code, UserManager<TUser> manager, TUser user)
        {
            var topt = await _totpSercurityToken.Where(x => x.Code == code && x.Email == purpose && x.Type == _passwordReset)
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