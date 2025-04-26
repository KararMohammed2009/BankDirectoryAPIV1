using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using FluentValidation;

namespace BankDirectoryApi.API.Validators.Related.Auth
{
    /// <summary>
    /// Validator for the UpdateUserDTO class.
    /// </summary>
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDTO>
    {
        public UpdateUserDtoValidator()
        {
            // Id: Required
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");

            // UserName: Optional, but if provided, must be between 3-50 characters and only letters, numbers, and underscores.
            RuleFor(x => x.UserName)
                .Length(3, 50).WithMessage("Username must be between 3 and 50 characters.")
                .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores.")
                .When(x => !string.IsNullOrEmpty(x.UserName));

            // Email: Optional, if provided, must be a valid email format.
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format.")
                .When(x => !string.IsNullOrEmpty(x.Email));

            // PhoneNumber: Optional, if provided, must be a valid international phone number format.
            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            // Password: Optional, but if provided, must meet the complexity requirements.
            RuleFor(x => x.Password)
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"\d").WithMessage("Password must contain at least one number.")
                .Matches(@"[\W_]").WithMessage("Password must contain at least one special character.")
                .When(x => !string.IsNullOrEmpty(x.Password));

            // RolesNames: Optional, if provided, should be a valid list.
            RuleFor(x => x.RolesNames)
                .Must(r => r == null || r.Any()).WithMessage("Roles must be a valid list or null.")
                .When(x => x.RolesNames != null);

            // TwoFactorEnabled: No validation needed, since it's a boolean.
        }
    }
}