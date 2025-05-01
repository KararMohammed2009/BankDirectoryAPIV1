using BankDirectoryApi.Application.DTOs.Core.Branches;
using BankDirectoryApi.Application.DTOs.Core.Cards;
using FluentValidation;

namespace BankDirectoryApi.API.Validators.Core.Card
{
    /// <summary>
    /// Validator for CardFilterDTO.
    /// </summary>
    public class CardFilterDTOValidator: AbstractValidator<CardFilterDTO>
    {
        public CardFilterDTOValidator()
        {
            // Card Name Validation
            RuleFor(card => card.CardName)
                .MaximumLength(100).WithMessage("Card name cannot exceed 100 characters.")
                .When(card => !string.IsNullOrEmpty(card.CardName)); // Only apply if card name is not empty
            // Card Type Validation
            RuleFor(card => card.CardType)
                .IsInEnum().WithMessage("Card type is invalid.")
                .When(card => card.CardType.HasValue); // Only apply if card type is not null
            // Annual Fee Validation
            RuleFor(card => card.AnnualFee)
                .GreaterThan(0).WithMessage("Annual fee must be a positive number.")
                .When(card => card.AnnualFee.HasValue); // Only apply if annual fee is not null
            // BankId Validation
            RuleFor(card => card.BankId)
                .GreaterThan(0).WithMessage("BankId must be a positive integer.")
                .When(card => card.BankId.HasValue); // Only apply if BankId is not null

        }
    }
}
