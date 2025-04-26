using BankDirectoryApi.Application.DTOs.Core.Card;
using FluentValidation;

namespace BankDirectoryApi.API.Validators.Core.Card
{
    /// <summary>
    /// Validator for the CardDTO class.
    /// </summary>
    public class CardDTOValidator :AbstractValidator<CardDTO>
    {
        public CardDTOValidator() {
            // BankId Validation
            RuleFor(card => card.BankId)
                .NotEmpty().WithMessage("BankId is required.")
                .GreaterThan(0).WithMessage("BankId must be a positive integer.");
            // Name Validation
            RuleFor(card => card.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
            // Type Validation
            RuleFor(card => card.Type)
                .NotNull().WithMessage("Type is required.")
                .IsInEnum().WithMessage("Type must be a valid CardType enum value.");
            // AnnualFee Validation
            RuleFor(card => card.AnnualFee)
                .GreaterThanOrEqualTo(0).WithMessage("Annual fee must be a non-negative number.")
                .When(card => card.AnnualFee.HasValue); 
        }
    }
}
