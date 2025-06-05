using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using DocumentService.Application.Dtos.Requests;
using DocumentService.Application.Dtos.Responses;

namespace AdminPanel.Mvc.Services.DictionaryServiceClient;

public class DictionaryServiceClient : IDictionaryServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DictionaryServiceClient> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DictionaryServiceClient(
        HttpClient httpClient,
        ILogger<DictionaryServiceClient> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<List<EducationDocumentTypeDto>> GetEducationDocumentTypes()
    {
        var url = $"api/Dictionary/documentTypes";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        
        var response = await _httpClient.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to check admission ownership. Status code: {StatusCode}", response.StatusCode);
            throw new BadHttpRequestException("Failed to get document types"); 
        }
        
        var content = await response.Content.ReadAsStringAsync();
        
        return JsonSerializer.Deserialize<List<EducationDocumentTypeDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        });
    }
}