using System.ComponentModel.DataAnnotations;

namespace DocumentService.Application.Dtos.Requests;

public class CreateEducationDocumentDto
{
    [Required]
    [MinLength(1)]
    public required string Name { get; set; }
    
    [Required]
    public required Guid EducationDocumentTypeId { get; set; }
}