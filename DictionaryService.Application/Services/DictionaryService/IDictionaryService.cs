using Contract.Application.Common;
using DictionaryService.Application.Dtos.Requests;
using DictionaryService.Application.Dtos.Responses;

namespace DictionaryService.Application.Services.DictionaryService;

public interface IDictionaryService
{
    Task<GenericResult<List<EducationLevelDto>>> GetEducationLevels();
    Task<GenericResult<List<EducationDocumentTypeDto>>> GetDocumentTypes();
    Task<GenericResult<List<FacultyDto>>> GetFaculties();
    Task<GenericResult<ProgramPagedListDto>> GetPrograms(FilterParametersDto parameters);
}