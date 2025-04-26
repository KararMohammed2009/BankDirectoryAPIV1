using BankDirectoryApi.Application.DTOs.Core.ATM;
using FluentValidation;

namespace BankDirectoryApi.API.Validators.Core.ATM
{
    /// <summary>
    /// Validator for the ATMDTO class.
    /// </summary>
    public class ATMDTOValidator :AbstractValidator<ATMDTO>
    {
        public ATMDTOValidator()
        {
            // BankId Validation
            RuleFor(atm => atm.BankId)
                .NotEmpty().WithMessage("BankId is required.")
                .GreaterThan(0).WithMessage("BankId must be a positive integer.");
            // GeoCoordinate Validation
            RuleFor(atm => atm.GeoCoordinate)
                .NotNull().WithMessage("GeoCoordinate is required.");
            // IsOperational Validation
            RuleFor(atm => atm.IsOperational)
                .NotNull().WithMessage("IsOperational is required.");

        }
    }
}
