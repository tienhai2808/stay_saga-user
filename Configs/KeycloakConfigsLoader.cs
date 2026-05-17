using Common.Exceptions;

namespace UserService.Configs;

public static class KeycloakConfigsLoader
{
    public static KeycloakConfigs LoadAndValidate(IConfiguration configuration)
    {
        var keycloakConfigs = configuration.GetSection("Keycloak").Get<KeycloakConfigs>()
            ?? throw new InternalServerException("Keycloak configuration section is required.");

        var missingFields = new List<string>();
        if (string.IsNullOrWhiteSpace(keycloakConfigs.Realm))
            missingFields.Add("Keycloak:Realm");
        if (string.IsNullOrWhiteSpace(keycloakConfigs.AdminUrl))
            missingFields.Add("Keycloak:AdminUrl");
        if (string.IsNullOrWhiteSpace(keycloakConfigs.AdminClientId))
            missingFields.Add("Keycloak:AdminClientId");
        if (string.IsNullOrWhiteSpace(keycloakConfigs.AdminClientSecret))
            missingFields.Add("Keycloak:AdminClientSecret");
        if (string.IsNullOrWhiteSpace(keycloakConfigs.ClientId))
            missingFields.Add("Keycloak:ClientId");
        if (string.IsNullOrWhiteSpace(keycloakConfigs.ClientSecret))
            missingFields.Add("Keycloak:ClientSecret");

        if (missingFields.Count > 0)
            throw new InternalServerException(
                $"Missing required Keycloak configuration fields: {string.Join(", ", missingFields)}");

        return keycloakConfigs;
    }
}
