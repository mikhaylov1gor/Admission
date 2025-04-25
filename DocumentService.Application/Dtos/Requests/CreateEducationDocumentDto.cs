namespace DocumentService.Application.Dtos.Requests;

public class CreateEducationDocumentDto
{
    public required string Name { get; set; }
    public required Guid DocumentTypeId { get; set; }
}