using Swashbuckle.AspNetCore.Annotations;

namespace BankDirectoryApi.Application.DTOs.Core.Branch
{
    /// <summary>
    /// Data Transfer Object (DTO) representing a branch update.
    /// </summary>
    public class BranchUpdateDTO :BranchDTO
    {
        [SwaggerSchema("The unique identifier of the branch.", Nullable = false)]
        public new int Id { get; set; }

    }
}
