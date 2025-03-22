using System.ComponentModel.DataAnnotations;

namespace ems_backend.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string Phone { get; set; }

        public DateTime Dob { get; set; }

        [Required]
        public string Password { get; set; } // Hashed password

        public List<Ticket> TicketsOwned { get; set; } = new();
        public List<Event> EventsHosted { get; set; } = new();
    }
}
