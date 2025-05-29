using Contract.Application.Validations;
using Contract.Domain.Enums;

namespace Contract.Dtos.Dtos.Requests;

public class GetAdmissionsDto
{
    public AdmissionStatus? AdmissionStatus {get; set;}
    public bool? IsManagerExists {get; set;}
    public bool OnlyMine {get; set;}
    public Sorting Sorting {get; set;}
    [Page]
    public int Page {get; set;}
    [Size]
    public int Size {get; set;}
}