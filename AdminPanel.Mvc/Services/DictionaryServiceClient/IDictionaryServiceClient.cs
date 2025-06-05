using DocumentService.Application.Dtos.Requests;
using DocumentService.Application.Dtos.Responses;

namespace AdminPanel.Mvc.Services.DictionaryServiceClient;

public interface IDictionaryServiceClient
{
    Task<List<EducationDocumentTypeDto>> GetEducationDocumentTypes();
}