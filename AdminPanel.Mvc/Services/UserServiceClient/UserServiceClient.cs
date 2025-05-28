using AdminPanel.Mvc.Models.Dtos;

namespace AdminPanel.Mvc.Services.UserServiceClient;

public class UserServiceClient : IUserServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserServiceClient> _logger;

    public UserServiceClient(
        HttpClient httpClient,
        ILogger<UserServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    public async Task<AuthResponse> Login(string email, string password)
    {
        try
        {
            var loginRequest = new { Email = email, Password = password };
            var response = await _httpClient.PostAsJsonAsync("api/Applicant/login", loginRequest);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Login failed: {errorContent}");
                return null;
            }

            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return null;
        }
    }
    
    public async Task<AuthResponse> RefreshTokens(string refreshToken)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Applicant/refreshToken", new { refreshToken });
        
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Refresh token failed: {errorContent}");
            }

            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return null;
        }
    }

}