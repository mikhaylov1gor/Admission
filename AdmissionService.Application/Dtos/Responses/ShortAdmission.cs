using Contract.Domain.Enums;

namespace AdmissionService.Application.Dtos.Responses;

public class ShortAdmission
{
    public Guid Id { get; set; }
    public AdmissionStatus Status { get; init; }
    public bool IsManagerExist { get; init; }
}