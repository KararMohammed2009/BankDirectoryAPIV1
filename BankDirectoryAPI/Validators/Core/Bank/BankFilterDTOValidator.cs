using BankDirectoryApi.Application.DTOs.Core.Banks;
using FluentValidation;

namespace BankDirectoryApi.API.Validators.Core.Bank
{
    /// <summary>
    /// Validator for the BankFilterDTO class.
    /// </summary>
    public class BankFilterDTOValidator: AbstractValidator<BankFilterDTO>
    {
        public BankFilterDTOValidator()
        {
            // Bank Name Validation
            RuleFor(x => x.BankName)
                .MaximumLength(100)
                .When(x => !string.IsNullOrEmpty(x.BankName)) // Only apply if bank name is not empty
                .WithMessage("Bank name cannot exceed 100 characters.");
            // Customer Support Number Validation
            RuleFor(x => x.CustomerSupportNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Customer support number is invalid.")
                .When(x => !string.IsNullOrEmpty(x.CustomerSupportNumber)); // Only apply if phone number is not empty
            // Bank Code Validation
            RuleFor(x => x.BankCode)
                .Matches(@"^[A-Z]{3,8}$").WithMessage("Bank code must be 3 to 8 letters.")
                .When(x => !string.IsNullOrEmpty(x.BankCode)); // Only apply if code is not empty
        }
    }
}
