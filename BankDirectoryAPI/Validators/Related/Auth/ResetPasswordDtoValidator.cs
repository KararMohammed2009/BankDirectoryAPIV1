using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using FluentValidation;

namespace BankDirectoryApi.API.Validators.Related.Auth
{
    public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDTO>
    {
        public ResetPasswordDtoValidator()
        {
            // Email: Required, valid email format.
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            // Code: Required, not empty.
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.");

            // NewPassword: Required, must meet the password complexity requirements.
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(8).WithMessage("New password must be at least 8 characters.")
                .Matches(@"[A-Z]").WithMessage("New password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("New password must contain at least one lowercase letter.")
                .Matches(@"\d").WithMessage("New password must contain at least one number.")
                .Matches(@"[\W_]").WithMessage("New password must contain at least one special character.");
        }
    }
}
