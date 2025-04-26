using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using FluentValidation;

namespace BankDirectoryApi.API.Validators.Related.Auth
{
    public class ForgotPasswordDtoValidator : AbstractValidator<ForgotPasswordDTO>
    {
        public ForgotPasswordDtoValidator()
        {
            // Email: Required, valid email format.
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
        }
    }
}
