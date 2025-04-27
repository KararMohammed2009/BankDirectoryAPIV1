using BankDirectoryApi.Application.DTOs.Core.ATMs;
using FluentValidation;

namespace BankDirectoryApi.API.Validators.Core.ATM
{
    /// <summary>
    /// Validator for the ATMUpdateDTO class.
    /// </summary>
    public class ATMUpdateDTOValidator:AbstractValidator<ATMUpdateDTO>
    {
        public ATMUpdateDTOValidator()
        {
            // Id Validation
            RuleFor(atm => atm.Id)
                .NotEmpty().WithMessage("Id is required.")
                .GreaterThan(0).WithMessage("Id must be a positive integer.");
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
