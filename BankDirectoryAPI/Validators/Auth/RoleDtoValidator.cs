using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using FluentValidation;

namespace BankDirectoryApi.API.Validators.Auth
{
    public class RoleDtoValidator : AbstractValidator<RoleDTO>
    {
        /// <summary>
        /// Validator for RoleDTO.
        /// </summary>
        public RoleDtoValidator()
        {
            // UserId: Required, Guid format
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            // RoleName: Required, not empty
            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("RoleName is required.");
        }
    }
}
