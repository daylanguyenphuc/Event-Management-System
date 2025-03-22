using ems_backend.Data;
using ems_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace ems_backend.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get user by ID with event participation details
        public async Task<UserDTO?> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    Firstname = u.FirstName,
                    Lastname = u.LastName,
                    Email = u.Email,
                    Phone = u.Phone,
                    Dob = u.Dob,
                    ParticipatedEvents = _context.Tickets.Count(t => t.UserId == userId && t.Status == TicketStatus.Redeemed),
                    HostedEvents = _context.Events.Count(e => e.OwnerId == userId && !e.IsCanceled)
                })
                .FirstOrDefaultAsync();
        }

        // Update user profile
        public async Task<UserDTO?> UpdateUserProfileAsync(Guid userId, UserRequest updatedUser)
        {
            var existingUser = await _context.Users.FindAsync(userId);
            if (existingUser == null) return null;

            existingUser.FirstName = updatedUser.Firstname;
            existingUser.LastName = updatedUser.Lastname;
            existingUser.Email = updatedUser.Email;
            existingUser.Phone = updatedUser.Phone;
            existingUser.Dob = updatedUser.Dob;

            await _context.SaveChangesAsync();

            return new UserDTO
            {
                Id = existingUser.Id,
                Firstname = existingUser.FirstName,
                Lastname = existingUser.LastName,
                Email = existingUser.Email,
                Phone = existingUser.Phone,
                Dob = existingUser.Dob,
                ParticipatedEvents = await _context.Tickets.CountAsync(t => t.UserId == userId && t.Status == TicketStatus.Redeemed),
                HostedEvents = await _context.Events.CountAsync(e => e.OwnerId == userId && !e.IsCanceled)
            };
        }
    }

    // DTO
    public class UserRequest
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime Dob { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

    }

    public class UserDTO
    {
        public Guid Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime Dob { get; set; }
        public int ParticipatedEvents { get; set; }
        public int HostedEvents { get; set; }

    }
}
