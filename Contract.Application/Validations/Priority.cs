using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Contract.Application.Validations;

public class Priority: ValidationAttribute
{
    private const int maxPriority = 5;
    
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null) 
            return new ValidationResult("Priority cannot be null.");

        if (value is int intValue)
        {
            if (intValue >= 1 && intValue <= maxPriority)
                return ValidationResult.Success;
            else
                return new ValidationResult("Page Size must be greater than 0 and less than {maxPriority}.");
        }

        return new ValidationResult("Value type is not supported.");
    }
}