using System.Text.Json;
using DocumentService.Application.Dtos.Responses;
using Microsoft.Extensions.Logging;

namespace DocumentService.Application.Services.AdmissionServiceClient;

public class AdmissionServiceClient : IAdmissionServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AdmissionServiceClient> _logger;

    public AdmissionServiceClient(HttpClient httpClient, ILogger<AdmissionServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> IsAdmissionOpen()
    {
        var response = await _httpClient.GetAsync($"/api/Admission/University/IsOpen");
        
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var content = await response.Content.ReadAsStringAsync();
        
        if (bool.TryParse(content, out var isOpen))
        {
            return isOpen;
        }

        _logger.LogError("Invalid response format: {Content}", content);
        return false;
    }
}