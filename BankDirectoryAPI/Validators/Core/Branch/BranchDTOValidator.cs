using BankDirectoryApi.Application.DTOs.Core.Branch;
using FluentValidation;

namespace BankDirectoryApi.API.Validators.Core.Branch
{
    /// <summary>
    /// Validator for the BranchDTO class.
    /// </summary>
    public class BranchDTOValidator :AbstractValidator<BranchDTO>
    {
        public BranchDTOValidator() {

            // BankId Validation
            RuleFor(branch => branch.BankId)
                .NotEmpty().WithMessage("BankId is required.")
                .GreaterThan(0).WithMessage("BankId must be a positive integer.");
            // Name Validation
            RuleFor(branch => branch.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
           
            // Customer Support Number Validation
            RuleFor(branch => branch.CustomerSupportNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Customer support number is invalid.")
                .When(branch => !string.IsNullOrEmpty(branch.CustomerSupportNumber)); // Only apply if phone number is not empty
        }
    }
}
