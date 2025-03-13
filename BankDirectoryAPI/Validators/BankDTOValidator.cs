using BankDirectoryApi.Application.DTOs.Core;
using FluentValidation;

namespace BankDirectoryApi.API.Validators
{
    public class BankDTOValidator : AbstractValidator<BankDTO>
    {
        public BankDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Bank name is required.")
                .Length(1, 100).WithMessage("Bank name must be between 1 and 100 characters.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.");

            RuleFor(x => x.NumberOfBranches)
                .InclusiveBetween(1, 1000).WithMessage("Number of branches must be between 1 and 100.");
        }
    }
}
