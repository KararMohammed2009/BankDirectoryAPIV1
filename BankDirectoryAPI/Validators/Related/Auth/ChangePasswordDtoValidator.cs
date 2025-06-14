﻿using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using FluentValidation;

namespace BankDirectoryApi.API.Validators.Related.Auth
{
    /// <summary>
    /// Validator for the ChangePasswordDTO class.
    /// </summary>
    public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDTO>
    {
        public ChangePasswordDtoValidator()
        {
            // Email: Required, valid email format.
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            // CurrentPassword: Required, not empty.
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required.");

            // NewPassword: Required, must meet the password complexity requirements.
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(8).WithMessage("New password must be at least 8 characters.")
                .Matches(@"[A-Z]").WithMessage("New password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("New password must contain at least one lowercase letter.")
                .Matches(@"\d").WithMessage("New password must contain at least one number.")
                .Matches(@"[\W_]").WithMessage("New password must contain at least one special character.")
                .NotEqual(x => x.CurrentPassword).WithMessage("New password must be different from the current password.");
        }
    }
}
