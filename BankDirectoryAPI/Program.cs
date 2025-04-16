using AspNetCoreRateLimit;
using BankDirectoryApi.API.Extensions;
using BankDirectoryApi.API.Middleware;
using BankDirectoryApi.Application.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BankDirectoryApi;
using System.Text;
using BankDirectoryApi.Common.Extensions;
using BankDirectoryApi.API.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(
    options =>
    {
        options.Filters.Add<CustomValidationFailureFilter>();
    });
builder.Services.AddApplicationMappers();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddGlobalMappers();

builder.AddTheSwagger();

builder.AddLimitRate();
builder.AddTheCors();
builder.AddTheAuthentication();
builder.AddTheAuthorization();
builder.AddTheValidators();
builder.AddTheVersioning();
builder.AddTheUserServices();
builder.AddTheSerilogLogger();
builder.AddJwtAuth();
builder.AddTheExternalServices();

var app = builder.Build();

app.UseTheSwagger();
app.UseRouting();
app.UseJwtAuth();
app.UseLimitRate();
app.UseExceptionMiddleware();
app.UseRequestLoggingMiddleware();
app.UseRateLimitLoggingMiddleware();
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseHttpsRedirection();
app.MapControllers();
//app.InitializeDatabase();
app.Run();
public partial class Program { }
