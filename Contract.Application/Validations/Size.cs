using System.ComponentModel.DataAnnotations;

namespace Contract.Application.Validations;

public class Size : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null) 
            return new ValidationResult("Page size can not be nullable.");

        if (value is int intValue)
        {
            if (intValue >= 1)
                return ValidationResult.Success;
            else
                return new ValidationResult("Page size can not be less than 1.");
        }
    }
}