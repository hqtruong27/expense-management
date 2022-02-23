using ExpenseManagement.Api.IocConfig;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                 new OpenApiSecurityScheme
                 {
                   Reference = new OpenApiReference
                   {
                     Type = ReferenceType.SecurityScheme,
                     Id = "Bearer"
                   }
                  },
                  Array.Empty<string>()
                }
              });
});

builder.Host.UseSerilog((hc, lc) =>
{
    lc.ReadFrom.Configuration(hc.Configuration)
               .Enrich.FromLogContext()
               .Enrich.WithProperty("ApplicationName", typeof(Program).Assembly.GetName().Name)
               .Enrich.WithProperty("Environment", hc.HostingEnvironment);
#if DEBUG
    // Used to filter out potentially bad data due debugging.
    // Very useful when doing Seq dashboards and want to remove logs under debugging session.
    lc.Enrich.WithProperty("DebuggerAttached", System.Diagnostics.Debugger.IsAttached);
#endif
});

builder.Register();



var app = builder.Build();

app.UseCorsExtensions();

//Auto update migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ExpenseManagement.Api.Data.ExpenseManagementDbcontext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}


app.UseSwagger();
app.UseSwaggerUI();
//app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();