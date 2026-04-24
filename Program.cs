using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Extensions;
using UserService.Middleware;
using UserService.Providers;
using UserService.Repositories;
using UserDomainService = UserService.Services.UserService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSnowflakeIdGenerator(builder.Configuration);
builder.Services.AddApiControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
  options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserDomainService>();
builder.Services.AddHttpClient<KeycloakProvider>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
