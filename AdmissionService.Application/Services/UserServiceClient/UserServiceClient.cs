using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AdmissionService.Application.Services.UserServiceClient;

public class UserServiceClient : IUserServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<DictionaryServiceClient.DictionaryServiceClient> _logger;

    public UserServiceClient(HttpClient httpClient, 
        ILogger<DictionaryServiceClient.DictionaryServiceClient> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<string> GetUserEmail(Guid userId)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
            .ToString()
            .Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("JWT token not found in request headers");
            return null;
        }
        
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync($"/api/Admin/User/{userId}/Email");
        
        Console.WriteLine($"EMAIL: {await response.Content.ReadAsStringAsync()}");
            
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to get user email. Status code: {StatusCode}", response.StatusCode);
            return null;
        }
        
        return await response.Content.ReadAsStringAsync();
    }
}