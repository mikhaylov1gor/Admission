using System.ComponentModel.DataAnnotations;

namespace Contract.Application.Validations;

public class Page : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null) 
            return new ValidationResult("Page can not be nullable.");

        if (value is int intValue)
        {
            if (intValue >= 1)
                return ValidationResult.Success;
            else
                return new ValidationResult("Page can not be less than 1.");
        }
        
        return new ValidationResult("Value type is not supported.");
    }
}