using Asp.Versioning;
using BankDirectoryApi.API.Helpers;
using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankDirectoryApi.API.Controllers.AuthControllers
{

    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Password")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RoleController : ControllerBase

    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        // assign user role
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] RoleDTO model)
        {
            var result = await _roleService.AssignRoleAsync(model.UserId, model.RoleName);
            return Ok(result);
        }
        // remove user role
        [HttpPost("remove-role")]
        public async Task<IActionResult> RemoveRole([FromBody] RoleDTO model)
        {
            var result = await _roleService.RemoveRoleAsync(model.UserId, model.RoleName);
            return Ok(result);
        }
         
    }
}
