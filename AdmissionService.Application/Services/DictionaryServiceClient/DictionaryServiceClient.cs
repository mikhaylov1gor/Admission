using System.Text.Json;
using AdmissionService.Application.Dtos.Responses;
using Contract.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace AdmissionService.Application.Services.DictionaryServiceClient;

public class DictionaryServiceClient : IDictionaryServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DictionaryServiceClient> _logger;

    public DictionaryServiceClient(HttpClient httpClient, ILogger<DictionaryServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<EducationProgramDto?> GetEducationProgramById(Guid programId)
    {
        var response = await _httpClient.GetAsync($"/api/Dictionary/program/{programId}");
        
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<EducationProgramDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public async Task<EducationLevelDto?> GetEducationLevelById(Guid educationLevelId)
    {
        var response = await _httpClient.GetAsync($"/api/Dictionary/educationLevel/{educationLevelId}");
        
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<EducationLevelDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}