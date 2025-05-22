using System.ComponentModel.DataAnnotations;
using Contract.Application.Validations;

namespace AdmissionService.Application.Dtos.Requests;

public class EditProgramsDto
{
    [Required]
    [Length(1,Priority.MaxPriority)]
    public required IEnumerable<EditProgramDto> EditPrograms { get; init; }
}