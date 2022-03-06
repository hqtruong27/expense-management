using ExpenseManagement.Api.Data;
using ExpenseManagement.Api.Data.Models;
using ExpenseManagement.Api.Data.Repositories;
using ExpenseManagement.Api.Hubs;
using ExpenseManagement.Api.Infrastructure;
using ExpenseManagement.Api.mapper;
using ExpenseManagement.Api.Mfa;
using ExpenseManagement.Api.Model;
using ExpenseManagement.Api.Options;
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text.Json.Serialization;

namespace ExpenseManagement.Api.IocConfig
{
    public static class IocConfig
    {
        private static readonly string _corsPolicy = "CorsPolicy";
        public static WebApplicationBuilder Register(this WebApplicationBuilder builder)
        {
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            var connecttionString = builder.Configuration.GetConnectionString("ExpenseManagementDbcontext");
            builder.Services.AddDbContextPool<ExpenseManagementDbcontext>(options =>
                     options.UseSqlServer(connecttionString));

            // Identity services
            builder.Services.AddIdentity<User, Role>()
                            .AddEntityFrameworkStores<ExpenseManagementDbcontext>()
                            .AddDefaultTokenProviders();

            builder.Services.AddSignalR();
            builder.Services.AddHangfire(c => c.UseSqlServerStorage(connecttionString));
            builder.Services.AddHangfireServer();

            builder.Services.AddAutoMapper(typeof(ServiceProfile).Assembly);
            builder.Services.AddHttpClient();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddSingleton<IFacebookService, FacebookService>();
            builder.Services.AddSingleton<IGoogleService, GoogleService>();
            builder.Services.AddSingleton<ISmsService, SmsService>();
            builder.Services.AddSingleton<IUserIdProvider, NameUserIdProvider>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
            builder.Services.AddScoped<IUserExpenseRepository, UserExpenseRepository>();
            builder.Services.AddScoped<IDebtChargeRepository, DebtChargeRepository>();
            builder.Services.AddScoped<IDebtReminderRepository, DebtReminderRepository>();
            builder.Services.AddScoped<ITransactionHistoryRepository, TransactionHistoryRepository>();
            builder.Services.AddScoped<ITransactionHistoryRepository, TransactionHistoryRepository>();
            builder.Services.AddScoped<IUserClaimsPrincipalFactory<User>, AdditionalUserClaimsPrincipalFactory>();
            builder.Services.AddScoped<IHangfireService, HangfireService>();

            // Add cors extensions
            builder.Services.AddCors(option => option.AddPolicy(_corsPolicy, x => x
                                                     .AllowAnyMethod()
                                                     .AllowAnyHeader()
                                                     .AllowCredentials()
                                                     .WithOrigins(builder.Configuration["AllowedOrigins"])));

            // Adding Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    LifetimeValidator = (before, expires, token, param) => expires > DateTime.UtcNow,
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = builder.Configuration["Authentication:JWT:ValidAudience"],
                    ValidIssuer = builder.Configuration["Authentication:JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Authentication:JWT:Secret"]))
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/notificationhub") || path.StartsWithSegments("/chathub")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };

            });
            //.AddOpenIdConnect(options =>
            //{
            //    options.SignInScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.Authority = "<OpenID Connect server URL>";
            //    options.RequireHttpsMetadata = true;
            //    options.ClientId = "<OpenID Connect client ID>";
            //    options.ClientSecret = "<>";
            //    // Code with PKCE can also be used here
            //    options.ResponseType = "code id_token";
            //    options.Scope.Add("profile");
            //    options.Scope.Add("offline_access");
            //    options.SaveTokens = true;
            //    options.Events = new OpenIdConnectEvents
            //    {
            //        OnRedirectToIdentityProvider = context =>
            //        {
            //            context.ProtocolMessage.SetParameter("acr_values", "mfa");
            //            return Task.FromResult(0);
            //        }
            //    };
            //});

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
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            builder.Services.Configure<IdentityOptions>(option =>
            {
                option.Password.RequireUppercase = false;
                option.Password.RequireLowercase = false;
                option.Password.RequireNonAlphanumeric = false;
                option.User.RequireUniqueEmail = true;
                //option.SignIn.RequireConfirmedEmail = true;
            });
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Expense Management Api",
                    Version = "v1",
                    Description = "This is spending management api",
                    Contact = new OpenApiContact
                    {
                        Email = "hqtruong27@gmail.com",
                        Name = "Truong Hoang",
                    }
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "JWT Authentication",
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Type = SecuritySchemeType.Http,
                    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                 new OpenApiSecurityScheme
                 {
                   Reference = new OpenApiReference
                   {
                     Id = "Bearer",
                     Type = ReferenceType.SecurityScheme,
                   }
                  }, Array.Empty<string>()
                }
              });
            });

            var configuration = builder.Configuration
                                       .SetBasePath(Directory.GetCurrentDirectory())
                                       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                       .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environments.Production}.json", optional: true)
                                       .Build();

            builder.Host.UseSerilog((ctx, lc) => lc
                        .ReadFrom.Configuration(configuration)
                        .Enrich.FromLogContext());

            //Serilog.Debugging.SelfLog.Enable(msg =>
            //{
            //    Debug.Print(msg);
            //    Debugger.Break();
            //});

            builder.Services.AddSingleton(builder.Configuration.GetSection(nameof(Email)).Get<Email>());
            builder.Services.AddSingleton(builder.Configuration.GetSection(nameof(Template)).Get<Template>());
            builder.Services.AddSingleton(builder.Configuration.GetSection(nameof(Authentication)).Get<Authentication>());

            return builder;
        }

        public static IApplicationBuilder UseCorsExtensions(this WebApplication app)
        {
            // configure HTTP request pipeline
            {
                // global cors policy
                app.UseCors(_corsPolicy);

                // custom jwt auth middleware
                //app.UseMiddleware<JwtMiddleware>();

                return app;
            }
        }

        public static IApplicationBuilder UseSwaggerExtensions(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Expense Management Api");
                c.RoutePrefix = "";
            });

            return app;
        }
    }
}