using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using FluentValidation;

namespace BankDirectoryApi.API.Validators.Related.Auth
{
    public class UserFilterDtoValidator : AbstractValidator<UserFilterDTO>
    {
        /// <summary>
        /// Validator for UserFilterDTO.
        /// </summary>
        public UserFilterDtoValidator()
        {
            // UserName: Optional, but if provided, should be within a reasonable length
          

            // EmailAddress: Optional
          

            // PhoneNumber: Optional, but if provided, should be a valid phone number format
          

            // RoleName: Optional, but if provided, should be within a reasonable length
          
            // PaginationInfo: Optional, but if provided, should have valid properties
            RuleFor(x => x.PaginationInfo)
                .SetValidator(new PaginationInfoValidator()) 
                .When(x => x.PaginationInfo != null);

            // OrderingInfo: Optional, no specific validation on the dictionary itself for now
            // You could add rules based on expected keys or value formats if needed.
        }
    }
    }
