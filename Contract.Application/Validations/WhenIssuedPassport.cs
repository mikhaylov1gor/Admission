using System.ComponentModel.DataAnnotations;

namespace Contract.Application.Validations;

public class WhenIssuedPassport : ValidationAttribute
{
    private const int MinimumAge = 14;
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null) 
            return new ValidationResult("Issued date cannot be null.");

        if (value is DateTime dateTime)
        {
            if (dateTime >= DateTime.Now)
                return new ValidationResult("Issued date cannot be in the future.");
            if (dateTime > DateTime.UtcNow - TimeSpan.FromDays(MinimumAge * 365))
                return new ValidationResult($"Your age must be grater than {MinimumAge}.");
            return ValidationResult.Success;
        }
        
        return new ValidationResult("Value type is not supported.");
    }
}