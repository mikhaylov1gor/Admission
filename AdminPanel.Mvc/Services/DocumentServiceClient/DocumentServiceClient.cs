using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using DocumentService.Application.Dtos.Responses;

namespace AdminPanel.Mvc.Services.DocumentServiceClient;

public class DocumentServiceClient :  IDocumentServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DocumentServiceClient> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DocumentServiceClient(
        HttpClient httpClient,
        ILogger<DocumentServiceClient> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PassportDto?> GetPassport(Guid applicantId)
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

            var request = new HttpRequestMessage(HttpMethod.Get, $"api/Admin/Applicant/{applicantId}/Passport");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var passportDto = JsonSerializer.Deserialize<PassportDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });
            return passportDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error get applicant");
            throw;
        }
    }

    public async Task<EducationDocumentDto> GetEducationDocument(Guid applicantId)
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

            var request = new HttpRequestMessage(HttpMethod.Get, $"api/Admin/Applicant/{applicantId}/EducationDocument");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            
            _logger.LogDebug("API Response: {Content}", content);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var educationDocument = JsonSerializer.Deserialize<EducationDocumentDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });

            return educationDocument;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error get applicant");
            throw;
        }
    }

    public async Task<DownloadFileDto> DownloadScan(Guid scanId, Guid applicantId)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                        .FirstOrDefault()?.Split(" ").Last()
                    ?? _httpContextAccessor.HttpContext?.Request.Cookies["AccessToken"];

        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("JWT token not found in headers or cookies");
            throw new UnauthorizedAccessException("Authentication token is missing");
        }
        
        var request = new HttpRequestMessage(
            HttpMethod.Get, 
            $"api/Admin/Applicant/{applicantId}/Scan/{scanId}");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);

        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to download scan. Status: {StatusCode}", response.StatusCode);
            throw new HttpRequestException($"Failed to download scan. Status: {response.StatusCode}");
        }
        
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
        var fileName = response.Content.Headers.ContentDisposition?.FileName 
                       ?? response.Content.Headers.ContentDisposition?.FileNameStar
                       ?? $"scan_{scanId}.bin";

        var fileData = await response.Content.ReadAsByteArrayAsync();

        return new DownloadFileDto
        {
            FileName = fileName,
            ContentType = contentType,
            Data = fileData
        };
    }
}