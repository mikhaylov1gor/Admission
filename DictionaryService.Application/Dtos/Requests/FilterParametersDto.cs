using System.ComponentModel.DataAnnotations;
using Contract.Application.Validations;

namespace DictionaryService.Application.Dtos.Requests;

public class FilterParametersDto
{
    public string? FacultyName { get; set; }
    public string? EducationLevelName { get; set; }
    public string? EducationForm { get; set; }
    public string? Language { get; set; }
    public string? Name { get; set; }
    
    [Required]
    [Page]
    public required int Page { get; set; } = 1;
    
    [Required]
    [Size]
    public required int Size { get; set; } = 5;

}