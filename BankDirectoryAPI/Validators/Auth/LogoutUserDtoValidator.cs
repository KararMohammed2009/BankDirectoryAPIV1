using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using FluentValidation;

namespace BankDirectoryApi.API.Validators.Auth
{
    public class LogoutUserDtoValidator: AbstractValidator<LogoutUserDTO>
    {
        public LogoutUserDtoValidator() {
            RuleFor(user => user.SessionId).NotNull().NotEmpty()
                .WithMessage("Valid Session Id is required");
        }
    }
}
