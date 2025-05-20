using System.Text.Json;
using DocumentService.Application.Dtos.Responses;
using Microsoft.Extensions.Logging;

namespace DocumentService.Application.Services.DictionaryServiceClient;

public class DictionaryServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DictionaryServiceClient> _logger;

    public DictionaryServiceClient(HttpClient httpClient, ILogger<DictionaryServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<EducationDocumentTypeDto?> GetEducationDocumentTypeAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"/education-document-types/{id}");
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("DirectoryService returned {StatusCode} for ID {Id}", response.StatusCode, id);
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<EducationDocumentTypeDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}