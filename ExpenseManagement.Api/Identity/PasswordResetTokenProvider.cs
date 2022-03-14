using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

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
                TokenLifespan = TimeSpan.FromDays(3);
            }
        }
    }
}
