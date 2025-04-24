using Contract.Application.Common;
using Contract.Application.Exceptions;
using DictionaryService.Application.Dtos.Requests;
using DictionaryService.Application.Dtos.Responses;

namespace DictionaryService.Application.Services.DictionaryService;

public class DictionaryService : IDictionaryService
{
    public DictionaryService()
    {
        
    }

    public async Task<GenericResult<List<EducationLevelDto>>> GetEducationLevels()
    {
        throw new NotFoundException("not found");
    }

    public async Task<GenericResult<List<EducationDocumentTypeDto>>> GetDocumentTypes()
    {
        throw new NotFoundException("not found");
    }

    public async Task<GenericResult<List<FacultyDto>>> GetFaculties()
    {
        throw new NotFoundException("not found");
    }

    public async Task<GenericResult<ProgramPagedListDto>> GetPrograms(FilterParametersDto parameters)
    {
        throw new NotFoundException("not found");
    }
}