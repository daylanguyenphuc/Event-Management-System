using ems_backend.Models;
using ems_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace ems_backend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // User Signup
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest newUser)
        {
            var result = await _authService.SignupAsync(newUser);
            if (result == null) return BadRequest("Email already in use.");
            return Ok(result);
        }

        // User Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var user = await _authService.LoginAsync(loginRequest.Email, loginRequest.Password);
            if (user == null) return Unauthorized("Invalid email or password.");
            return Ok(user);
        }

        // Change Password
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var success = await _authService.ChangePasswordAsync(request.UserId, request.CurrentPassword, request.NewPassword);
            if (!success) return BadRequest("Invalid current password.");
            return Ok("Password changed successfully.");
        }
    }
}

