using System.Text.Json;
using UserService.Exceptions;

namespace UserService.Providers;

public class KeycloakProvider(HttpClient httpClient, IConfiguration config)
{
  private readonly HttpClient _httpClient = httpClient;
  private readonly IConfiguration _config = config;

  public async Task<string> CreateUserAsync(string email, string password, string fullName)
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
      firstName = fullName,
      enabled = true,
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
      throw new ExternalServiceException($"Không thể kết nối Keycloak: {ex.Message}");
    }

    if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
      throw new ConflictException("Email đã tồn tại trên hệ thống định danh");

    if (!response.IsSuccessStatusCode)
      throw new ExternalServiceException($"Tạo user trên Keycloak thất bại. Status: {(int)response.StatusCode}");

    var location = response.Headers.Location?.ToString();
    var keycloakId = location?.Split('/').Last();

    return keycloakId ?? throw new ExternalServiceException("Không lấy được Keycloak ID");
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
        })
      );
    }
    catch (HttpRequestException ex)
    {
      throw new ExternalServiceException($"Không thể lấy admin token từ Keycloak: {ex.Message}");
    }

    if (!response.IsSuccessStatusCode)
      throw new ExternalServiceException($"Lấy admin token thất bại. Status: {(int)response.StatusCode}");

    var data = await response.Content.ReadFromJsonAsync<JsonElement>();
    if (!data.TryGetProperty("access_token", out var tokenElement))
      throw new ExternalServiceException("Không tìm thấy access_token từ Keycloak");

    var token = tokenElement.GetString();
    if (string.IsNullOrWhiteSpace(token))
      throw new ExternalServiceException("access_token rỗng từ Keycloak");

    return token;
  }

  private string GetRequiredConfig(string key)
  {
    var value = _config[key];
    if (string.IsNullOrWhiteSpace(value))
      throw new AppException($"Thiếu cấu hình bắt buộc: {key}", 500);

    return value;
  }
}
