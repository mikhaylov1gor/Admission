namespace DocumentService.Domain.Entities;

public class Passport : Document
{
    public required int Serial { get; set; }
    public required int Number { get; set; }
    public required string Hometown { get; set; }
    public required DateTime WhenIssued { get; set; }
    public required string WhoIssued { get; set; }
}