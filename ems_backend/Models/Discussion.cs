using System.ComponentModel.DataAnnotations.Schema;

namespace ems_backend.Models
{
    public class Discussion
    {
        public Guid Id { get; set; }
        
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Photos { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
        public DateTime? UpdatedAt { get; set; }

        public Guid EventId { get; set; }  // Linking discussion to an event
        [ForeignKey("EventId")]
        public Event Event { get; set; }  // Navigation property
        public Guid UserId { get; set; }  // Linking discussion to an user
        [ForeignKey("UserId")]
        public User User { get; set; }  // Navigation property
    }

}
