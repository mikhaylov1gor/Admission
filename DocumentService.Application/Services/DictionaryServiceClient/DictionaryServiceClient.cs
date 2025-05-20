using System.Text.Json;
using DocumentService.Application.Dtos.Responses;
using Microsoft.Extensions.Logging;

namespace DocumentService.Application.Services.DictionaryServiceClient;

public class DictionaryServiceClient : IDictionaryServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DictionaryServiceClient> _logger;

    public DictionaryServiceClient(HttpClient httpClient, ILogger<DictionaryServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<EducationDocumentTypeDto>?> GetEducationDocumentTypesAsync()
    {
        var response = await _httpClient.GetAsync($"/api/Dictionary/documentTypes");
        
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<EducationDocumentTypeDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}