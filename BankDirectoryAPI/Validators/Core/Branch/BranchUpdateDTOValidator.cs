using BankDirectoryApi.Application.DTOs.Core.Branch;
using FluentValidation;

namespace BankDirectoryApi.API.Validators.Core.Branch
{
    /// <summary>
    /// Validator for the BranchUpdateDTO class.
    /// </summary>
    public class BranchUpdateDTOValidator:AbstractValidator<BranchUpdateDTO>
    {
        public BranchUpdateDTOValidator() {
            // Id Validation
            RuleFor(branch => branch.Id)
                .NotEmpty().WithMessage("Id is required.")
                .GreaterThan(0).WithMessage("Id must be a positive integer.");
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
