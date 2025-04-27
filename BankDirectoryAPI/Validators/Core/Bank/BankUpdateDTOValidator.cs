using BankDirectoryApi.Application.DTOs.Core.Banks;
using FluentValidation;

namespace BankDirectoryApi.API.Validators.Core.Bank
{
    /// <summary>
    /// Validator for the BankUpdateDTO class.
    /// </summary>
    public class BankUpdateDTOValidator: AbstractValidator<BankUpdateDTO>
    {
        public BankUpdateDTOValidator()
        {
            // Id Validation
            RuleFor(bank => bank.Id)
                .NotEmpty().WithMessage("Id is required.")
                .GreaterThan(0).WithMessage("Id must be greater than 0.");
            // Name Validation
            RuleFor(bank => bank.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
            // Code Validation
            RuleFor(bank => bank.Code)
                .NotEmpty().WithMessage("Code is required.")
                .Matches(@"^[A-Z]{3,5}$").WithMessage("Code must be 3 to 5 uppercase letters.");
            // Website Validation
            RuleFor(bank => bank.Website)
                .Matches(@"^(http|https)://[^\s/$.?#].[^\s]*$").WithMessage("Website URL is invalid.")
                .When(bank => !string.IsNullOrEmpty(bank.Website)); // Only apply if website is not empty
            // Customer Support Number Validation
            RuleFor(bank => bank.CustomerSupportNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Customer support number is invalid.")
                .When(bank => !string.IsNullOrEmpty(bank.CustomerSupportNumber)); // Only apply if phone number is not empty
        }
        }
}
