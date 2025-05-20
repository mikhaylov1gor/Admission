using DocumentService.Application.Dtos.Responses;

namespace DocumentService.Application.Services.DictionaryServiceClient;

public interface IDictionaryServiceClient
{
    Task<List<EducationDocumentTypeDto>?> GetEducationDocumentTypesAsync();
}