namespace Contract.Dtos.Dtos.Responses;

public class AdmissionsDto
{
    public List<AdmissionDto> StudentAdmissions { get; set; }
    public Pagination Pagination { get; set; }
}