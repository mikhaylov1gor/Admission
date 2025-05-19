namespace DocumentService.Application.Dtos.Responses;

public class EducationDocumentTypeDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public EducationLevelDto EducationLevel { get; set; } = null!;

    public ICollection<EducationLevelDto> NextEducationLevels { get; } = new List<EducationLevelDto>();
}