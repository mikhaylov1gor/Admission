using Contract.Application.Common;
using Contract.Application.Exceptions;
using DictionaryService.Application.Dtos.Requests;
using DictionaryService.Application.Dtos.Responses;
using DictionaryService.Domain.Entities;
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

    public async Task <EducationProgramDto> GetProgramById(Guid programId)
    {
        var program = await _educationProgramRepository.GetProgramById(programId);

        if (program == null)
        {
            throw new NotFoundException("Program not found");
        }

        var faculty = new FacultyDto
        {
            Id = program.Faculty.Id,
            CreateTime = program.Faculty.CreateTime,
            Name = program.Faculty.Name,
        };

        var educationLevel = new EducationLevelDto
        {
            Id = program.EducationLevel.Id,
            Name = program.EducationLevel.Name,
        };

        var programDto = new EducationProgramDto
        {
            Id = program.Id,
            CreateTime = program.CreateTime,
            Name = program.Name,
            Code = program.Code,
            Language = program.Language,
            EducationForm = program.EducationForm,
            Faculty = faculty,
            EducationLevel = educationLevel,
        };
        
        return programDto;
    }

    public async Task<EducationLevelDto> GetEducationLevelById(Guid educationLevelId)
    {
        var educationLevel = await _educationLevelRepository.GetByIdAsync(educationLevelId);

        if (educationLevel == null)
        {
            throw new NotFoundException("Education level not found");
        }

        var educationLevelDto = new EducationLevelDto
        {
            Id = educationLevel.Id,
            Name = educationLevel.Name
        };
        
        return educationLevelDto;
    }
}