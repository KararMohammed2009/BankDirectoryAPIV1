using BankDirectoryApi.Application.DTOs.Core.ATMs;
using FluentValidation;

namespace BankDirectoryApi.API.Validators.Core.ATM
{
    /// <summary>
    /// Validator for ATMFilterDTO.
    /// </summary>
    public class ATMFilterDTOValidator : AbstractValidator<ATMFilterDTO>
    {
        public ATMFilterDTOValidator()
        {
            // BankId Validation
            RuleFor(atm => atm.BankId)
                .GreaterThan(0).WithMessage("BankId must be a positive integer.")
                .When(atm => atm.BankId.HasValue); // Only apply if BankId is not null
            // Geo Coordinate Validation
            RuleFor(atm => atm.GeoCoordinate)
                .Matches(@"^[-+]?\d{1,3}\.\d+,\s*[-+]?\d{1,3}\.\d+$").WithMessage("Geo coordinate must be in the format 'latitude,longitude'.")
                .When(atm => !string.IsNullOrEmpty(atm.GeoCoordinate)); // Only apply if geo coordinate is not empty
            // IsOperational Validation
            RuleFor(atm => atm.IsOperational)
                .NotNull().WithMessage("IsOperational cannot be null.")
                .When(atm => atm.IsOperational.HasValue); // Only apply if IsOperational is not null
        }
    }
}
