using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Providers;
using UserService.Repositories;
using UserDomainService = UserService.Services.UserService;
using Common.Extensions;

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
builder.Services.AddScoped<UserDomainService>();
builder.Services.AddHttpClient<KeycloakProvider>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseStaySagaDefaults();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
