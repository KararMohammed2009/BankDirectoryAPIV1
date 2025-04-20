using Asp.Versioning;
using BankDirectoryApi.API.Mappings.Interfaces;
using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankDirectoryApi.API.Controllers.AuthControllers
{
    /// <summary>
    /// Controller for handling role-related operations such as assigning and removing roles from users.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Password")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RoleController : ControllerBase

    {
        private readonly IRoleService _roleService;
        private readonly IActionGlobalMapper _actionGlobalMapper;
        /// <summary>
        /// Constructor for RoleController.
        /// </summary>
        /// <param name="roleService"></param>
        public RoleController(IRoleService roleService,
            IActionGlobalMapper actionGlobalMapper)
        {
            _roleService = roleService;
            _actionGlobalMapper = actionGlobalMapper;
        }
        /// <summary>
        /// Handles assigning a role to a user.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>The result of the role assignment process.</returns>
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] RoleDTO model)
        {
            var result = await _roleService.AssignRoleAsync(model.UserId, model.RoleName);
            return _actionGlobalMapper.MapResultToApiResponse(result);
        }
        /// <summary>
        /// Handles removing a role from a user.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>The result of the role removal process.</returns>
        [HttpPost("remove-role")]
        public async Task<IActionResult> RemoveRole([FromBody] RoleDTO model)
        {
            var result = await _roleService.RemoveRoleAsync(model.UserId, model.RoleName);
            return _actionGlobalMapper.MapResultToApiResponse(result);
        }
         
    }
}
