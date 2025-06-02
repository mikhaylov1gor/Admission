namespace DocumentService.Application.Dtos.Responses;

public class PassportDto
{
    public Guid Id { get; set; }
    public DateTime CreatedTime {get; set;}
    public DateTime? ModifiedTime {get; set;}
    
    public int Serial {get; set;}
    public int Number {get; set;}
    public string Hometown {get; set;}
    public DateTime WhenIssued {get; set;}
    public string WhoIssued {get; set;}
    public List<FileDto>? Files { get; set; }
    
}