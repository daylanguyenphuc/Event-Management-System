using ems_backend.Data;
using ems_backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ems_backend.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> SignupAsync(SignupRequest newUserRequest)
        {
            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == newUserRequest.Email))
                return null; // Email already in use

            // Convert SignupRequest DTO to User Model
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                FirstName = newUserRequest.Firstname,
                LastName = newUserRequest.Lastname,
                Dob = newUserRequest.Dob,
                Email = newUserRequest.Email,
                Phone = newUserRequest.Phone,
                Password = "" // Placeholder for hashed password
            };

            // Hash password before saving
            newUser.Password = _passwordHasher.HashPassword(newUser, newUserRequest.Password);

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return newUser;
        }


        // User Login
        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return null;

            // Verify password
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            return result == PasswordVerificationResult.Success ? user : null;
        }

        // Change Password
        public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            // Verify current password
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, currentPassword);
            if (result != PasswordVerificationResult.Success) return false;

            // Hash and update new password
            user.Password = _passwordHasher.HashPassword(user, newPassword);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    // DTOs (Data Transfer Objects)
    public class SignupRequest
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime Dob { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }

    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class ChangePasswordRequest
    {
        public Guid UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
