namespace DocumentService.Application.Dtos.Responses;

public class PassportDto
{
    public Guid Id { get; set; }
    public required DateTime CreatedTime {get; set;}
    public required DateTime? ModifiedTime {get; set;}
    
    public required int Serial {get; set;}
    public required int Number {get; set;}
    public required string Hometown {get; set;}
    public required DateTime WhenIssued {get; set;}
    public required string WhoIssued {get; set;}
    public List<FileDto>? Files { get; set; }
    
}