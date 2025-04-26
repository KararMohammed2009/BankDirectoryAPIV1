using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using FluentValidation;

namespace BankDirectoryApi.API.Validators.Related.Auth
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDTO>
    {
        public RegisterUserDtoValidator()
        {
            // UserName: Optional, but if provided, must be between 3-50 characters and only letters, numbers, and underscores.
            RuleFor(x => x.UserName)
                .Length(3, 50).WithMessage("Username must be between 3 and 50 characters.")
                .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores.")
                .When(x => !string.IsNullOrEmpty(x.UserName));

            // Email or PhoneNumber: At least one must be provided, and both should follow their respective formats.
            RuleFor(x => x.Email)
                .NotEmpty().When(x => string.IsNullOrEmpty(x.PhoneNumber)).WithMessage("Either Email or PhoneNumber is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.")
                .When(x => string.IsNullOrEmpty(x.Email) && !string.IsNullOrEmpty(x.PhoneNumber));

            // Password: Required, at least 8 characters, must contain an uppercase, lowercase, number, and special character.
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"\d").WithMessage("Password must contain at least one number.")
                .Matches(@"[\W_]").WithMessage("Password must contain at least one special character.");

           

            // TwoFactorEnabled: No validation needed, since it's a boolean.
        }
    }

}
