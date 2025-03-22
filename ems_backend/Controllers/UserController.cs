using ems_backend.Models;
using ems_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace ems_backend.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null) return NotFound("User not found.");
            return Ok(user);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUserProfile(Guid userId, [FromBody] UserRequest updatedUser)
        {
            var result = await _userService.UpdateUserProfileAsync(userId, updatedUser);
            if (result == null) return NotFound("User not found.");
            return Ok(result);
        }
    }
}

