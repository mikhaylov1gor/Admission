using System.Text.Json;
using AdmissionService.Application.Dtos.Responses;
using Microsoft.Extensions.Logging;

namespace AdmissionService.Application.Services.DocumentServiceClient;

public class DocumentServiceClient : IDocumentServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DictionaryServiceClient.DictionaryServiceClient> _logger;

    public DocumentServiceClient(HttpClient httpClient, ILogger<DictionaryServiceClient.DictionaryServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> GetEducationDocumentLevelName(Guid programId)
    {
        var response = await _httpClient.GetAsync($"/api/Document/Education");
        
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<dynamic>(content);
        
        return json?.educationDocumentType.educationLevel.name?.ToString();
    }
}