using System.Text.Json;
using UserService.DTOs;
using Common.Exceptions;

namespace UserService.Providers;

public class KeycloakProvider(HttpClient httpClient, IConfiguration config)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IConfiguration _config = config;

    public async Task<string> CreateUserAsync(string email, string password, string firstName, string lastName)
    {
        var token = await GetAdminTokenAsync();
        var realm = GetRequiredConfig("Keycloak:Realm");
        var adminUrl = GetRequiredConfig("Keycloak:AdminUrl");
        var request = new HttpRequestMessage(HttpMethod.Post, $"{adminUrl}/admin/realms/{realm}/users");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(new
        {
            email,
            username = email,
            firstName,
            lastName,
            enabled = true,
            emailVerified = true,
            credentials = new[]
            {
                new { type = "password", value = password, temporary = false }
            }
        });

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(request);
        }
        catch (HttpRequestException ex)
        {
            throw new ExternalServiceException($"Unable to connect to Keycloak: {ex.Message}");
        }

        if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            throw new ConflictException("Email already exists in identity provider");

        if (!response.IsSuccessStatusCode)
            throw new ExternalServiceException($"Failed to create user in Keycloak. Status: {(int)response.StatusCode}");

        var location = response.Headers.Location?.ToString();
        var keycloakId = location?.Split('/').Last();

        return keycloakId ?? throw new ExternalServiceException("Unable to retrieve Keycloak user ID");
    }

    public async Task<AuthResponseDto> LoginAsync(string email, string password)
    {
        var realm = GetRequiredConfig("Keycloak:Realm");
        var adminUrl = GetRequiredConfig("Keycloak:AdminUrl");
        var clientId = GetRequiredConfig("Keycloak:ClientId");
        var clientSecret = GetRequiredConfig("Keycloak:ClientSecret");

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.PostAsync(
                $"{adminUrl}/realms/{realm}/protocol/openid-connect/token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type", "password" },
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "username", email },
                    { "password", password }
                }));
        }
        catch (HttpRequestException ex)
        {
            throw new ExternalServiceException($"Unable to connect to Keycloak: {ex.Message}");
        }

        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest
            || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException("Email or password is incorrect");
        }

        if (!response.IsSuccessStatusCode)
            throw new ExternalServiceException($"Keycloak login failed. Status: {(int)response.StatusCode}");

        var data = await response.Content.ReadFromJsonAsync<JsonElement>();
        return new AuthResponseDto(
        GetRequiredString(data, "access_token"),
        GetRequiredString(data, "refresh_token"),
        GetRequiredInt(data, "expires_in"),
        GetRequiredInt(data, "refresh_expires_in")
        );
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var realm = GetRequiredConfig("Keycloak:Realm");
        var adminUrl = GetRequiredConfig("Keycloak:AdminUrl");
        var clientId = GetRequiredConfig("Keycloak:ClientId");
        var clientSecret = GetRequiredConfig("Keycloak:ClientSecret");

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.PostAsync(
                $"{adminUrl}/realms/{realm}/protocol/openid-connect/logout",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "refresh_token", refreshToken }
                })
);
        }
        catch (HttpRequestException ex)
        {
            throw new ExternalServiceException($"Unable to connect to Keycloak: {ex.Message}");
        }

        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest
            || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException("Refresh token is invalid or expired");
        }

        if (!response.IsSuccessStatusCode)
            throw new ExternalServiceException($"Keycloak logout failed. Status: {(int)response.StatusCode}");
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var realm = GetRequiredConfig("Keycloak:Realm");
        var adminUrl = GetRequiredConfig("Keycloak:AdminUrl");
        var clientId = GetRequiredConfig("Keycloak:ClientId");
        var clientSecret = GetRequiredConfig("Keycloak:ClientSecret");

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.PostAsync(
                $"{adminUrl}/realms/{realm}/protocol/openid-connect/token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type", "refresh_token" },
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "refresh_token", refreshToken }
                }));
        }
        catch (HttpRequestException ex)
        {
            throw new ExternalServiceException($"Unable to connect to Keycloak: {ex.Message}");
        }

        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest
            || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException("Refresh token is invalid or expired");
        }

        if (!response.IsSuccessStatusCode)
            throw new ExternalServiceException($"Keycloak refresh token failed. Status: {(int)response.StatusCode}");

        var data = await response.Content.ReadFromJsonAsync<JsonElement>();
        return new AuthResponseDto(
            GetRequiredString(data, "access_token"),
            GetRequiredString(data, "refresh_token"),
            GetRequiredInt(data, "expires_in"),
            GetRequiredInt(data, "refresh_expires_in")
        );
    }

    private async Task<string> GetAdminTokenAsync()
    {
        var realm = GetRequiredConfig("Keycloak:Realm");
        var adminUrl = GetRequiredConfig("Keycloak:AdminUrl");
        var clientId = GetRequiredConfig("Keycloak:AdminClientId");
        var clientSecret = GetRequiredConfig("Keycloak:AdminClientSecret");

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.PostAsync(
                $"{adminUrl}/realms/{realm}/protocol/openid-connect/token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" },
                    { "client_id", clientId },
                    { "client_secret", clientSecret }
                }));
        }
        catch (HttpRequestException ex)
        {
            throw new ExternalServiceException($"Unable to get admin token from Keycloak: {ex.Message}");
        }

        if (!response.IsSuccessStatusCode)
            throw new ExternalServiceException($"Failed to get admin token. Status: {(int)response.StatusCode}");

        var data = await response.Content.ReadFromJsonAsync<JsonElement>();
        if (!data.TryGetProperty("access_token", out var tokenElement))
            throw new ExternalServiceException("Missing access_token in Keycloak response");

        var token = tokenElement.GetString();
        if (string.IsNullOrWhiteSpace(token))
            throw new ExternalServiceException("access_token is empty in Keycloak response");

        return token;
    }

    private static string GetRequiredString(JsonElement data, string key)
    {
        if (!data.TryGetProperty(key, out var element))
            throw new ExternalServiceException($"Missing '{key}' field in Keycloak response");

        var value = element.GetString();
        if (string.IsNullOrWhiteSpace(value))
            throw new ExternalServiceException($"'{key}' is empty in Keycloak response");

        return value;
    }

    private static int GetRequiredInt(JsonElement data, string key)
    {
        if (!data.TryGetProperty(key, out var element))
            throw new ExternalServiceException($"Missing '{key}' field in Keycloak response");

        if (!element.TryGetInt32(out var value))
            throw new ExternalServiceException($"'{key}' is not a valid integer in Keycloak response");

        return value;
    }

    private string GetRequiredConfig(string key)
    {
        var value = _config[key];
        if (string.IsNullOrWhiteSpace(value))
            throw new HttpException("INTERNAL_ERROR", $"Missing required configuration: {key}", 500);

        return value;
    }
}
