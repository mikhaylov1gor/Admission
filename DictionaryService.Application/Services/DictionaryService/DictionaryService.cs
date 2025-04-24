using Contract.Application.Common;
using Contract.Application.Exceptions;
using DictionaryService.Application.Dtos.Requests;
using DictionaryService.Application.Dtos.Responses;
using DictionaryService.Domain.IRepositories;

namespace DictionaryService.Application.Services.DictionaryService;

public class DictionaryService : IDictionaryService
{
    private readonly IEducationLevelRepository _educationLevelRepository;
    private readonly IDocumentTypeRepository _documentTypeRepository;
    private readonly IFacultyRepository _facultyRepository;
    private readonly IEducationProgramRepository _educationProgramRepository;
    
    public DictionaryService(
        IEducationLevelRepository educationLevelRepository,
        IDocumentTypeRepository documentTypeRepository,
        IFacultyRepository facultyRepository,
        IEducationProgramRepository educationProgramRepository)
    {
        _educationLevelRepository = educationLevelRepository;
        _documentTypeRepository = documentTypeRepository;
        _facultyRepository = facultyRepository;
        _educationProgramRepository = educationProgramRepository;
    }

    public async Task<GenericResult<List<EducationLevelDto>>> GetEducationLevels()
    {
        var resultDb = await _educationLevelRepository.GetAllAsync();
        
        var resultDto = resultDb
            .Select(el => new EducationLevelDto
            {
                Id = el.Id,
                Name = el.Name,
            })
            .ToList();

        return GenericResult<List<EducationLevelDto>>.Success(resultDto);
    }

    public async Task<GenericResult<List<EducationDocumentTypeDto>>> GetDocumentTypes()
    {
        var resultDb = await _documentTypeRepository.GetAllAsync();
        
        var resultDto = resultDb
            .Select(edt => new EducationDocumentTypeDto
            {
                Id = edt.Id,
                CreateTime = edt.CreateTime,
                Name = edt.Name,
                EducationLevel = new EducationLevelDto
                {
                    Id = edt.EducationLevel.Id,
                    Name = edt.EducationLevel.Name,
                },
                NextEducationLevels = edt.NextEducationLevels?
                    .Select(nel => new EducationLevelDto
                    {
                        Id = nel.Id,
                        Name = nel.Name,
                    })
                    .ToList()
            })
            .ToList();

        return GenericResult<List<EducationDocumentTypeDto>>.Success(resultDto);
    }

    public async Task<GenericResult<List<FacultyDto>>> GetFaculties()
    {
        var resultDb = await _facultyRepository.GetAllAsync();
        
        var resultDto = resultDb
            .Select(f => new FacultyDto
            {
                Id = f.Id,
                CreateTime = f.CreateTime,
                Name = f.Name,
            })
            .ToList();

        return GenericResult<List<FacultyDto>>.Success(resultDto);
    }

    public async Task<GenericResult<ProgramPagedListDto>> GetPrograms(FilterParametersDto parameters)
    {
        var resultDb = await _educationProgramRepository.GetProgramsByParameters(
            parameters.FacultyName,
            parameters.EducationLevelName,
            parameters.EducationForm,
            parameters.Language,
            parameters.Name);

        var totalCount = resultDb.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / parameters.Size);

        var pagedItems = resultDb
            .OrderBy(p => p.Name)
            .Skip((parameters.Page - 1) * parameters.Size)
            .Take(parameters.Size)
            .ToList();

        var items = pagedItems.Select(p => new EducationProgramDto
        {
            Id = p.Id,
            CreateTime = p.CreateTime,
            Name = p.Name,
            Code = p.Code,
            Language = p.Language,
            EducationForm = p.EducationForm,
            Faculty = new FacultyDto
            {
                Id = p.Faculty.Id,
                CreateTime = p.Faculty.CreateTime,
                Name = p.Faculty.Name,
            },
            EducationLevel = new EducationLevelDto
            {
                Id = p.EducationLevel.Id,
                Name = p.EducationLevel.Name,
            }
        }).ToList();
        
        return GenericResult<ProgramPagedListDto>.Success(new ProgramPagedListDto
        {
            EducationPrograms = items,
            Pagination = new PageInfoDto
            {
                Size = parameters.Size,
                Count = totalPages,
                Current = parameters.Page,
            }
        });
    }
}