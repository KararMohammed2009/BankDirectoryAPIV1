using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using FluentValidation;

namespace BankDirectoryApi.API.Validators.Auth
{
    public class RegisterUserDtoValidator: AbstractValidator<RegisterUserDTO>
    {
        RegisterUserDtoValidator()
        {
            // UserName: Required, between 3-50 characters, only letters, numbers, and underscores.
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.")
                .Length(3, 50).WithMessage("Username must be between 3 and 50 characters.")
                .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores.");

            // Email: Required, valid email format.
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            // Password: Required, at least 8 characters, must contain an uppercase, lowercase, number, and special character.
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"\d").WithMessage("Password must contain at least one number.")
                .Matches(@"[\W_]").WithMessage("Password must contain at least one special character.");

            // Roles: Required, must contain at least one role.
            RuleFor(x => x.Roles)
                .NotNull().WithMessage("Roles are required.")
                .Must(r => r != null && r.Any()).WithMessage("At least one role is required.");

            // PhoneNumber: Optional but must be a valid phone number if provided.
            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("Invalid phone number format.")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            // TwoFactorEnabled: No validation needed, since it's a boolean.
        }
    }
}
