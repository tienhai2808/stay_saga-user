using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace UserService.Extensions;

public static class AuthExtensions
{
  public static IServiceCollection AddKeycloakJwtAuth(this IServiceCollection services, IConfiguration configuration)
  {
    var authority = GetRequiredConfig(configuration, "Keycloak:Authority");
    var audience = GetRequiredConfig(configuration, "Keycloak:Audience");

    services
      .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
      {
        options.Authority = authority;
        options.RequireHttpsMetadata = authority.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
        options.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuer = true,
          ValidIssuer = authority,
          ValidateAudience = true,
          ValidAudience = audience,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true
        };
      });

    services.AddAuthorization();
    return services;
  }

  private static string GetRequiredConfig(IConfiguration configuration, string key)
  {
    var value = configuration[key];
    if (string.IsNullOrWhiteSpace(value))
      throw new InvalidOperationException($"Missing required configuration: {key}");

    return value;
  }
}
