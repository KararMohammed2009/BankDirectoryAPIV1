using BankDirectoryApi.Application.DTOs.Related.Communications;
using FluentValidation;

namespace BankDirectoryApi.API.Validators.Related.Auth
{
    /// <summary>
    /// Validator for EmailConfirmationDTO.
    /// </summary>
    public class EmailConfirmationDtoValidator : AbstractValidator<EmailConfirmationDTO>
    {
        public EmailConfirmationDtoValidator()
        {
            // Email: Required, valid email format.
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            // Token: Required, not empty.
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token is required.");
        }
    }
}
