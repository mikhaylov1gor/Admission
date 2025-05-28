using AdminPanel.Mvc.Models.Dtos;
using AdminPanel.Mvc.Models.Dtos.Requests;
using Microsoft.AspNetCore.Http;

namespace AdminPanel.Mvc.Services.UserServiceClient;

public class UserServiceClient : IUserServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserServiceClient> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserServiceClient(
        HttpClient httpClient,
        ILogger<UserServiceClient> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    private void SetAuthorizationHeader()
    {
        var accessToken = _httpContextAccessor.HttpContext?.Request.Cookies["AccessToken"];
        if (!string.IsNullOrEmpty(accessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        }
    }

    public async Task<AuthResponse> Login(string email, string password)
    {
        try
        {
            var loginRequest = new { Email = email, Password = password };
            var response = await _httpClient.PostAsJsonAsync("api/User/login", loginRequest);
            
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
            var response = await _httpClient.PostAsJsonAsync("api/User/refreshToken", new { refreshToken });
        
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

    public async Task<ChangePasswordResult> ChangePassword(string currentPassword, string newPassword)
    {
        try
        {
            SetAuthorizationHeader();
            
            var request = new
            {
                oldPassword = currentPassword,
                newPassword = newPassword
            };

            var response = await _httpClient.PutAsJsonAsync("api/User/resetPassword", request);
        
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Change password failed: {errorContent}");
                return new ChangePasswordResult { Success = false, ErrorMessage = errorContent };
            }

            return new ChangePasswordResult { Success = true };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling change password API");
            return new ChangePasswordResult { Success = false, ErrorMessage = "Произошла ошибка при смене пароля" };
        }
    }
    
}