using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Contract.Application.Validations;

public class PhoneNumberAttribute : ValidationAttribute
{
    private static readonly Regex PhoneRegex = 
        new Regex(@"^(\+7 \(\d{3}\) \d{3}-\d{2}-\d{2}|((\+7|8)\d{10}))$", RegexOptions.Compiled);

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null) 
            return ValidationResult.Success;

        if (value is string stringValue)
        {
            if (PhoneRegex.IsMatch(stringValue))
                return ValidationResult.Success;
            else
                return new ValidationResult("Phone number is not valid.");
        }

        return new ValidationResult("Value type is not supported.");
    }
}