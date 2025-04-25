namespace DocumentService.Application.Dtos.Requests;

public class EditEducationDocumentDto
{
    public required string Name { get; set; }
    public required Guid DocumentTypeId { get; set; }
}