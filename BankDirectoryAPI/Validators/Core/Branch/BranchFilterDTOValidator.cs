using BankDirectoryApi.Application.DTOs.Core.Banks;
using BankDirectoryApi.Application.DTOs.Core.Branches;
using FluentValidation;

namespace BankDirectoryApi.API.Validators.Core.Branch
{
    /// <summary>
    /// Validator for BranchFilterDTO.
    /// </summary>
    public class BranchFilterDTOValidator: AbstractValidator<BranchFilterDTO>
    {
        public BranchFilterDTOValidator() {
            // Name Validation
            RuleFor(branch => branch.BranchName)

                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.")
                .When(branch => !string.IsNullOrEmpty(branch.BranchName)); // Only apply if branch name is not empty

            // Customer Support Number Validation
            RuleFor(branch => branch.CustomerSupportNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Customer support number is invalid.")
                .When(branch => !string.IsNullOrEmpty(branch.CustomerSupportNumber)); // Only apply if phone number is not empty
            // Address Validation
            RuleFor(branch => branch.Address)
                .MaximumLength(200).WithMessage("Address cannot exceed 200 characters.")
                .When(branch => !string.IsNullOrEmpty(branch.Address)); // Only apply if address is not empty
            // Geo Coordinate Validation
            RuleFor(branch => branch.GeoCoordinate)
                .Matches(@"^[-+]?\d{1,3}\.\d+,\s*[-+]?\d{1,3}\.\d+$").WithMessage("Geo coordinate must be in the format 'latitude,longitude'.")
                .When(branch => !string.IsNullOrEmpty(branch.GeoCoordinate)); // Only apply if geo coordinate is not empty

            // BankId Validation
            RuleFor(branch => branch.BankId)
                .GreaterThan(0).WithMessage("BankId must be a positive integer.")
                .When(branch => branch.BankId.HasValue); // Only apply if BankId is not null

        }
    }
}
