using Asp.Versioning;
using UserDirectoryApi.Application.DTOs;
using UserDirectoryApi.Application.Interfaces;
using UserDirectoryApi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UserDirectoryApi.API.Controllers.AuthControllers
{
    [Authorize(Roles ="Admin")]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/User")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _UserService;
        public UserController(IUserService UserService)
        {
            _UserService = UserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
          
            var Users = await _UserService.GetAllUsersAsync();
            return Ok(Users);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var User = await _UserService.GetUserByIdAsync(id);
            if (User == null) return NotFound();
            return Ok(User);
        }
        [HttpGet("{id}/branches")]
        public async Task<IActionResult> GetUserWithBranches(int id)
        {
            var User = await _UserService.GetUserWithBranchesAsync(id);
            if (User == null) return NotFound();
            return Ok(User);
        }
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] UserDTO Userdto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _UserService.AddUserAsync(Userdto);
            return CreatedAtAction(nameof(GetUserById), new { id = Userdto.Id }, Userdto);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDTO Userdto)
        {
            if (id != Userdto.Id) return BadRequest();
            await _UserService.UpdateUserAsync(Userdto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _UserService.DeleteUserAsync(id);
            return NoContent();
        }
    }
}
