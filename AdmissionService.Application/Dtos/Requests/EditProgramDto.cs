using System.ComponentModel.DataAnnotations;
using Contract.Application.Validations;

namespace AdmissionService.Application.Dtos.Requests;

public class EditProgramDto
{
    [Required]
    [Priority]
    public int Priority { get; set; }
    
    [Required]
    public Guid EducationProgramId { get; set; }
}