using BankDirectoryApi.Domain.Classes.Pagination;
using FluentValidation;

namespace BankDirectoryApi.API.Validators.Auth
{
    public class PaginationInfoValidator : AbstractValidator<PaginationInfo>
    {
        public PaginationInfoValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1).WithMessage("Page number must be at least 1.")
                .When(x => x != null);

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1).WithMessage("Page size must be at least 1.")
                .LessThanOrEqualTo(10000000).WithMessage("Page size cannot exceed 10000000.") 
                .When(x => x != null);
        }
    }
}
