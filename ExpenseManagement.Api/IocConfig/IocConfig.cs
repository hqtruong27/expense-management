using ExpenseManagement.Api.Data;
using ExpenseManagement.Api.Data.Models;
using ExpenseManagement.Api.Data.Repositories;
using ExpenseManagement.Api.Infrastructure;
using ExpenseManagement.Api.Mfa;
using ExpenseManagement.Api.Model;
using ExpenseManagement.Api.Options;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

namespace ExpenseManagement.Api.IocConfig
{
    public static class IocConfig
    {
        public static void Register(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContextPool<ExpenseManagementDbcontext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("ExpenseManagementDbcontext")));
            // Identity services
            builder.Services.AddIdentity<User, Role>()
                    .AddEntityFrameworkStores<ExpenseManagementDbcontext>()
                    .AddDefaultTokenProviders();

            builder.Services.AddCors(option => option.AddPolicy("CorsPolicy", x => x.AllowAnyMethod()
                                             .AllowAnyHeader()
                                             .AllowAnyOrigin()));

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddHttpClient();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddSingleton<IFacebookService, FacebookService>();
            builder.Services.AddSingleton<IGoogleService, GoogleService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
            builder.Services.AddScoped<IUserExpenseRepository, UserExpenseRepository>();

            // Adding Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // Adding Jwt Bearer
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = builder.Configuration["Authentication:JWT:ValidAudience"],
                    ValidIssuer = builder.Configuration["Authentication:JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Authentication:JWT:Secret"]))
                };
            }).AddOpenIdConnect(options =>
            {
                options.SignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.Authority = "<OpenID Connect server URL>";
                options.RequireHttpsMetadata = true;
                options.ClientId = "<OpenID Connect client ID>";
                options.ClientSecret = "<>";
                // Code with PKCE can also be used here
                options.ResponseType = "code id_token";
                options.Scope.Add("profile");
                options.Scope.Add("offline_access");
                options.SaveTokens = true;
                options.Events = new OpenIdConnectEvents
                {
                    OnRedirectToIdentityProvider = context =>
                    {
                        context.ProtocolMessage.SetParameter("acr_values", "mfa");
                        return Task.FromResult(0);
                    }
                };
            });

            builder.Services.AddScoped<IUserClaimsPrincipalFactory<User>, AdditionalUserClaimsPrincipalFactory>();

            builder.Services.AddAuthorization(options => options.AddPolicy("TwoFactorEnabled", x => x.RequireClaim("amr", "mfa")));

            builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
            builder.Services.AddMvc(options =>
            {
                options.Filters.Add(typeof(Model.Validator.ValidationResultAttribute));
            }).AddFluentValidation(fv =>
            {
                fv.ImplicitlyValidateRootCollectionElements = true;
                fv.ImplicitlyValidateChildProperties = true;
                fv.RegisterValidatorsFromAssemblyContaining<CreateUserRequest>();
            }).AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            });

            builder.Services.Configure<IdentityOptions>(option =>
            {
                option.Password.RequireUppercase = false;
                option.Password.RequireLowercase = false;
                option.Password.RequireNonAlphanumeric = false;
                option.User.RequireUniqueEmail = true;
                //option.SignIn.RequireConfirmedEmail = true;
            });


            builder.Services.AddSingleton(builder.Configuration.GetSection(nameof(Authentication)).Get<Authentication>());
            builder.Services.AddSingleton(builder.Configuration.GetSection(nameof(Email)).Get<Email>());
        }

        public static void UseCorsExtensions(this WebApplication app)
        {
            // configure HTTP request pipeline
            {
                // global cors policy
                app.UseCors("CorsPolicy");

                // custom jwt auth middleware
                //app.UseMiddleware<JwtMiddleware>();

                //app.MapControllers();
            }
        }
    }
}