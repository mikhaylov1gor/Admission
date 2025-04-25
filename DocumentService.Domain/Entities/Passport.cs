namespace DocumentService.Domain.Entities;

public class Passport
{
    public Guid Id { get; private set; }
    public required DateTime CreatedTime {get; set;} = DateTime.UtcNow;
    public DateTime? ModifiedTime {get; set;}
    
    public required int Serial { get; set; }
    public required int Number { get; set; }
    public required string Hometown { get; set; }
    public required DateTime WhenIssued { get; set; }
    public required string WhoIssued { get; set; }
}