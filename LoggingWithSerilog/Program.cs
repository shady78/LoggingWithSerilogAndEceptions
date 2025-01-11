using LoggingWithSerilog.Exceptions;
using LoggingWithSerilog.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.SignalR.Protocol;
using Serilog;
using System.Net;
using System.Security.Cryptography;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting Server.");
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Services.AddExceptionHandler<ProductNotFoundExceptionHandler>();


    builder.Host.UseSerilog((context, loggerConfiguration) =>
    {
        loggerConfiguration.WriteTo.Console();
        loggerConfiguration.ReadFrom.Configuration(context.Configuration);
    });
    // Add services to the container.
    builder.Services.AddTransient<IDummyService, DummyService>();

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
    //app.MapGet("/", (IDummyService svc) => svc.DoSomething());
    app.MapGet("/", () => { throw new ProductNotFoundException(Guid.NewGuid()); });

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

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
