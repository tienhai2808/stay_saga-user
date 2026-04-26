using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Providers;
using UserService.Repositories;
using UserService.Services;
using Common.Extensions;
using Common.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSnowflakeIdGenerator(builder.Configuration);
builder.Services.AddApiControllers();
builder.Services.AddOpenApi();
builder.Services.AddKeycloakJwtAuth(builder.Configuration);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Default"),
        npgsqlOptions => npgsqlOptions.MigrationsHistoryTable("__ef_migrations_history")));
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddHttpClient<KeycloakProvider>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<HttpExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
