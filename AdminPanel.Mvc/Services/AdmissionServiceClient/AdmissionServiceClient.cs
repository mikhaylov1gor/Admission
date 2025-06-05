using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AdmissionService.Application.Dtos.Requests;
using AdmissionService.Application.Dtos.Responses;
using Contract.Application.Exceptions;
using Contract.Domain.Enums;
using Contract.Dtos.Dtos.Requests;
using Contract.Dtos.Dtos.Responses;

namespace AdminPanel.Mvc.Services.AdmissionServiceClient;

public class AdmissionServiceClient : IAdmissionServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AdmissionServiceClient> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AdmissionServiceClient(
        HttpClient httpClient,
        ILogger<AdmissionServiceClient> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AdmissionsDto?> GetAdmissions(GetAdmissionsDto admissionsDto)
    {
        try
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                token = _httpContextAccessor.HttpContext?.Request.Cookies["AccessToken"];
            }

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("JWT token not found in cookies");
                return null;
            }

            var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

            if (admissionsDto.AdmissionStatus.HasValue)
                queryString.Add("admissionStatus", admissionsDto.AdmissionStatus.Value.ToString());

            if (admissionsDto.IsManagerExists.HasValue)
                queryString.Add("isManagerExists", admissionsDto.IsManagerExists.Value.ToString().ToLower());

            queryString.Add("onlyMine", admissionsDto.OnlyMine.ToString().ToLower());
            queryString.Add("sorting", admissionsDto.Sorting.ToString());
            queryString.Add("page", "1");
            queryString.Add("size", "5");

            var url = $"api/Admin/Admissions/Get?{queryString}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"API request failed: {response.StatusCode}");
                return null;
            }

            return await response.Content.ReadFromJsonAsync<AdmissionsDto>(
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during getting admissions");
            return null;
        }
    }

    public async Task<bool> TakeUntakeAdmission(Guid admissionId, Guid applicantId)
    {
        try
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                token = _httpContextAccessor.HttpContext?.Request.Cookies["AccessToken"];
            }

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("JWT token not found in cookies");
                return false;
            }

            var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

            queryString.Add("applicantId", applicantId.ToString());

            var url = $"api/Admin/Admission/{admissionId}/TakeUntake?{queryString}";

            var request = new HttpRequestMessage(HttpMethod.Patch, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"API request failed: {response.StatusCode}");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during switching admission");
            return false;
        }

        return true;
    }

    public async Task<StudentAdmissionDto> GetStudentAdmission(Guid admissionId)
    {
        try
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                            .FirstOrDefault()?.Split(" ").Last()
                        ?? _httpContextAccessor.HttpContext?.Request.Cookies["AccessToken"];

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("JWT token not found in headers or cookies");
                throw new NotFoundException("JWT token not found in cookies");
            }

            var url = $"api/Admission/Admission/{admissionId}";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"API request failed with status code: {response.StatusCode}");
                throw new NotFoundException($"API request failed with status code: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<StudentAdmissionDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });

            if (result != null)
                return result;
            
            throw new NotFoundException(content);
        }
        catch (HttpRequestException httpEx)
        {
            _logger.LogError(httpEx, "HTTP request error during getting admission");
            throw new BadRequestException($"HTTP request error during getting admission");
        }
        catch (JsonException jsonEx)
        {
            _logger.LogError(jsonEx, "JSON deserialization error");
            throw new BadRequestException($"JSON deserialization error during getting admission");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during getting admission");
            throw new BadRequestException($"Unexpected error during getting admission");
        }
    }

    public async Task<bool> IsMineAdmission(Guid admissionId)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                        .FirstOrDefault()?.Split(" ").Last()
                    ?? _httpContextAccessor.HttpContext?.Request.Cookies["AccessToken"];

        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("JWT token not found in headers or cookies");
            throw new NotFoundException("JWT token not found in cookies");
        }
        
        var url = $"api/Admin/Admission/{admissionId}/IsMine";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to check admission ownership. Status code: {StatusCode}", response.StatusCode);
            return false; 
        }
        
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<bool>(content);
    }

    public async Task<bool> ChangeStatusAsync(Guid admissionId, AdmissionStatus newStatus)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                        .FirstOrDefault()?.Split(" ").Last()
                    ?? _httpContextAccessor.HttpContext?.Request.Cookies["AccessToken"];

        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("JWT token not found in headers or cookies");
            throw new NotFoundException("JWT token not found in cookies");
        }
        
        var url = $"api/Admin/Admission/{admissionId}/ChangeStatus?status={newStatus}";

        using var request = new HttpRequestMessage(HttpMethod.Put, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to change admission status. Status code: {StatusCode}", response.StatusCode);
            return false; 
        }

        return true;
    }

    public async Task<bool> RemoveProgramFromAdmissionAsync(Guid admissionId, Guid programId)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                        .FirstOrDefault()?.Split(" ").Last()
                    ?? _httpContextAccessor.HttpContext?.Request.Cookies["AccessToken"];

        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("JWT token not found in headers or cookies");
            throw new NotFoundException("JWT token not found in cookies");
        }
        
        var url = $"api/Admin/Admissions/{admissionId}/Programs/{programId}";

        using var request = new HttpRequestMessage(HttpMethod.Delete, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to change admission status. Status code: {StatusCode}", response.StatusCode);
            return false; 
        }

        return true;
    }

    public async Task<bool> ChangePriorities(Guid admissionId, EditProgramsDto dto)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                        .FirstOrDefault()?.Split(" ").Last()
                    ?? _httpContextAccessor.HttpContext?.Request.Cookies["AccessToken"];

        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("JWT token not found in headers or cookies");
            throw new NotFoundException("JWT token not found in cookies");
        }
        
        var url = $"api/Admin/Admissions/{admissionId}/Priorities";

        using var request = new HttpRequestMessage(HttpMethod.Put, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var jsonContent = JsonSerializer.Serialize(dto, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });
        
        Console.WriteLine($"dto : {jsonContent}");
        
        request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to change program priorities. Status code: {StatusCode}", response.StatusCode);
            return false; 
        }

        return true;
    }

    public async Task<bool> AssignManager(Guid admissionId, Guid managerId)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                        .FirstOrDefault()?.Split(" ").Last()
                    ?? _httpContextAccessor.HttpContext?.Request.Cookies["AccessToken"];

        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("JWT token not found in headers or cookies");
            throw new NotFoundException("JWT token not found in cookies");
        }
        
        var url = $"api/Admin/Admissions/{admissionId}/AssignManager/{managerId}";

        using var request = new HttpRequestMessage(HttpMethod.Put, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var response = await _httpClient.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to change program priorities. Status code: {StatusCode}", response.StatusCode);
            return false; 
        }

        return true;
    }
}