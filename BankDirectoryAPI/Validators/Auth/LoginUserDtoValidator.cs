using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using FluentValidation;

namespace BankDirectoryApi.API.Validators.Auth
{
    public class LoginUserDtoValidator : AbstractValidator<LoginUserDTO>
    {
        public LoginUserDtoValidator()
        {
            // Email Validation
            RuleFor(user => user.Email)
                .Cascade(CascadeMode.Stop) // Stop validation if empty
                .NotEmpty().WithMessage("Email, Phone Number, or Username is required.")
                .When(user => string.IsNullOrEmpty(user.PhoneNumber) && string.IsNullOrEmpty(user.UserName)) // Only apply if phone and username are empty
                .EmailAddress().WithMessage("Email is invalid.")
                .When(user => !string.IsNullOrEmpty(user.Email)); // Only apply if email is not empty

            // Phone Number Validation
            RuleFor(user => user.PhoneNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Email, Phone Number, or Username is required.")
                 .When(user => string.IsNullOrEmpty(user.Email) && string.IsNullOrEmpty(user.UserName))
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Phone number is invalid.")
                .When(user => !string.IsNullOrEmpty(user.PhoneNumber));

            // Username Validation
            RuleFor(user => user.UserName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Email, Phone Number, or Username is required.")
                .When(user => string.IsNullOrEmpty(user.Email) && string.IsNullOrEmpty(user.PhoneNumber))
                .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
                .MaximumLength(50).WithMessage("Username cannot exceed 50 characters.")
                .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores.")
                .When(user => !string.IsNullOrEmpty(user.UserName));

            // Password Validation
            RuleFor(user => user.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches(@"[A-Z]+").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]+").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"[0-9]+").WithMessage("Password must contain at least one number.")
                .Matches(@"[\!\?\*\.\@\#\$\%\^\&\(\)\_\+\-\=\[\]\{\}\|\;\:\'\""\,\\/\<\>\`\~]+").WithMessage("Password must contain at least one special character.");

        }
    }
}
