using System.ComponentModel.DataAnnotations;
using Contract.Application.Validations;

namespace DocumentService.Application.Dtos.Requests;

public class CreatePassportDto
{
    [Required]
    [Range(1000, 9999)]
    public required int Serial { get; set; }
    
    [Required]
    [Range(100000, 999999)]
    public required int Number { get; set; }
    
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public required string Hometown { get; set; }
    
    [Required]
    [WhenIssuedPassport]
    public required DateTime WhenIssued { get; set; }
    
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public required string WhoIssued { get; set; }
}