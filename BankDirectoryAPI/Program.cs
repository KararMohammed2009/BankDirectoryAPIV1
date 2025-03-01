using AspNetCoreRateLimit;
using BankDirectoryApi.API.Extensions;
using BankDirectoryApi.API.Middleware;
using BankDirectoryApi.Application.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BankDirectoryApi;
using System.Text;
using BankDirectoryApi.Common.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddApplicationMappers();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.AddTheSwagger();
builder.AddJwtAuth();
builder.AddLimitRate();
builder.AddTheCors();
builder.AddTheAuthentication();

var app = builder.Build();

app.UseTheSwagger();
app.UseJwtAuth();
app.UseLimitRate();
app.UseExceptionMiddleware();
app.UseRequestLoggingMiddleware();
app.UseRateLimitLoggingMiddleware();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
