namespace DocumentService.Application.Dtos.Responses;

public class EducationDocumentDto
{
    public Guid Id { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime? ModifiedTime { get; set; }
    
    public string Name { get; set; }
    public EducationDocumentTypeDto EducationDocumentType { get; set; }
    
    public ICollection<FileDto> Files { get; set; } = new List<FileDto>();
}