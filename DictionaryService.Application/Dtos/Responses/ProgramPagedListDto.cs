namespace DictionaryService.Application.Dtos.Responses;

public class ProgramPagedListDto
{
       public List<EducationProgramDto>? EducationPrograms { get; set; }
       public PageInfoDto? Pagination { get; set; }
}