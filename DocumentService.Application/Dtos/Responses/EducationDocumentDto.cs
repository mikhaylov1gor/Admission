namespace DocumentService.Application.Dtos.Responses;

public class EducationDocumentDto
{
    public Guid Id { get; set; }
    public required DateTime CreatedTime { get; set; }
    public DateTime? ModifiedTime { get; set; }
    public required string Name { get; set; }
}