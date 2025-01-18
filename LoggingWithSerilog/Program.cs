using FluentValidation;
using Hangfire;
using LoggingWithSerilog.Data;
using LoggingWithSerilog.Exceptions;
using LoggingWithSerilog.Models;
using LoggingWithSerilog.Services;
using LoggingWithSerilog.Settings;
using LoggingWithSerilog.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Hangfire.PostgreSql;


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting Server.");
    var builder = WebApplication.CreateBuilder(args);


  


    builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

    // Add Database Context
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.SignIn.RequireConfirmedEmail = true;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


    // Configure JWT authentication
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddTransient<IMailService, MailService>();

    //builder.Services.AddScoped<IEmailService, EmailService>();

    builder.Services.AddTransient<IProductService, ProductService>();

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    // In-Memory Caching
    builder.Services.AddMemoryCache();

    // Distributed Cash with Redis 
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = "localhost";
        options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions()
        {
            AbortOnConnectFail = true,
            EndPoints = { options.Configuration }
        };
    });


    builder.Services.AddExceptionHandler<ProductNotFoundExceptionHandler>();

    //builder.Services.AddScoped<IValidator<UserRegistrationRequest>, UserRegistrationValidator>();
    // In case your project has multiple validators, and you don’t want to register them manually one by one, you can use the following.
    builder.Services.AddValidatorsFromAssemblyContaining<UserRegistrationValidator>();


    builder.Host.UseSerilog((context, loggerConfiguration) =>
    {
        loggerConfiguration.WriteTo.Console();
        loggerConfiguration.ReadFrom.Configuration(context.Configuration);
    });
    // Add services to the container.
    builder.Services.AddTransient<IDummyService, DummyService>();



    

    // Add Hangfire Server
    builder.Services.AddHangfireServer();

    builder.Services.AddHangfire(x => x.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddHangfireServer();

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Default Exception Handling Middleware in .NET - UseExceptionHandler
    //app.UseExceptionHandler(options =>
    //    {
    //        options.Run(async context =>
    //        {
    //            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    //            context.Response.ContentType = "application/json";
    //            var exception = context.Features.Get<IExceptionHandlerFeature>();
    //            if (exception != null)
    //            {
    //                var message = $"{exception.Error.Message}";
    //                await context.Response.WriteAsync(message).ConfigureAwait(false);
    //            }
    //        });
    //});
    //Custom Middleware -Global Exception Handling [Old Method]
    //app.UseMiddleware<ErrorHandlerMiddleware>();



    app.UseExceptionHandler();


    app.UseSerilogRequestLogging();
    // Minimum API
    //app.MapGet("/", (IDummyService svc) => svc.DoSomething());
    app.MapGet("/", () => { throw new ProductNotFoundException(Guid.NewGuid()); });
    app.MapPost("/register", async (UserRegistrationRequest request, IValidator<UserRegistrationRequest> validator) =>
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }
        // perform actual service call to register the user to the system
        // _service.RegisterUser(request);
        return Results.Accepted();
    });

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Enable Hangfire Dashboard
    app.UseHangfireDashboard("/mydashboard");

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{

    Log.Fatal(ex, "server terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
