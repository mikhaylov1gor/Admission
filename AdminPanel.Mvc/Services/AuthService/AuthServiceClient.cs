namespace AdminPanel.Mvc.Services.AuthService;

public class AuthServiceClient : IAuthServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthServiceClient> _logger;

    public AuthServiceClient(
        HttpClient httpClient,
        ILogger<AuthServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<AuthResponse> LoginAsync(string email, string password)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Applicant/login", new
            {
                Email = email,
                Password = password
            });

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<AuthResponse>();
                return new AuthResponse(
                    true,
                    content.AccessToken,
                    content.RefreshToken);
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Login failed: {Error}", errorContent);
            return new AuthResponse(false, Errors: new[] { "Неверный email или пароль" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Auth service error");
            return new AuthResponse(false, Errors: new[] { "Ошибка соединения с сервисом" });
        }
    }
}

public record AuthResponse(
    bool Success,              
    string AccessToken = null,         
    string RefreshToken = null, 
    IEnumerable<string> Errors = null 
);