using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using AdminPanel.Mvc.Models.Dtos;
using UserService.Application.Dtos.Responses;

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
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (result == null)
            {
                _logger.LogError("Failed to deserialize refresh token response");
                return null;
            }

            return result;
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

    public async Task<List<ManagerDto>> GetAllManagers(bool isAllManagers)
    {
        try
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                            .FirstOrDefault()?.Split(" ").Last()
                        ?? _httpContextAccessor.HttpContext?.Request.Cookies["AccessToken"];

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("JWT token not found in headers or cookies");
                return new List<ManagerDto>();
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"api/Admin/Managers?isAllManagers={isAllManagers}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"API request failed with status code: {response.StatusCode}");
                return new List<ManagerDto>();
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<ManagerDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });

            return result ?? new List<ManagerDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting managers list");
            return new List<ManagerDto>();
        }
    }
    public async Task<List<ApplicantDto>> GetAllApplicants()
    {
        try
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                            .FirstOrDefault()?.Split(" ").Last()
                        ?? _httpContextAccessor.HttpContext?.Request.Cookies["AccessToken"];

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("JWT token not found in headers or cookies");
                return new List<ApplicantDto>();
            }

            var request = new HttpRequestMessage(HttpMethod.Get, "api/Admin/Applicants");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"API request failed with status code: {response.StatusCode}");
                return new List<ApplicantDto>();
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<ApplicantDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });

            return result ?? new List<ApplicantDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting applicants list");
            return new List<ApplicantDto>();
        }
    }
    
    public async Task<bool> AssignRole(Guid userId, string role)
    {
        try
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                            .FirstOrDefault()?.Split(" ").Last()
                        ?? _httpContextAccessor.HttpContext?.Request.Cookies["AccessToken"];

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("JWT token not found in headers or cookies");
                return false;
            }
            
            var request = new HttpRequestMessage(HttpMethod.Post, $"api/Admin/Users/{userId}/AssignRole?role={role}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"API request failed with status code: {response.StatusCode}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role");
            return false;
        }
    }

    public async Task<ApplicantDto> GetApplicant(Guid applicantId)
    {
        try
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                            .FirstOrDefault()?.Split(" ").Last()
                        ?? _httpContextAccessor.HttpContext?.Request.Cookies["AccessToken"];

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("JWT token not found in headers or cookies");
                throw new Exception("unauthorized");
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"api/Admin/Applicant/{applicantId}/Profile");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"API request failed with status code: {response.StatusCode}");
                throw new Exception($"{response.StatusCode}");
            }
            
            var content = await response.Content.ReadAsStringAsync();
            var applicantDto = JsonSerializer.Deserialize<ApplicantDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });
            
            return applicantDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error get applicant");
            throw;
        }
    }
}