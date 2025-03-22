using System.ComponentModel.DataAnnotations;

namespace ems_backend.Models
{
    public class EventCategory
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public List<Event> Events { get; set; } = new();
    }

}
