namespace DictionaryService.Application.Dtos.Responses;

public class EducationDocumentTypeDto
{
    public Guid Id { get; set; }
    public DateTime createTime { get; set; }
    public string Name { get; set; }
    public EducationLevelDto EducationLevel { get; set; }
    public List<EducationLevelDto>? EducationLevels { get; set; }
}