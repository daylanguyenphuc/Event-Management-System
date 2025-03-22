using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Sockets;

namespace ems_backend.Models
{
    public class Event
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TicketPrice { get; set; }
        public int TicketsLeft { get; set; }

        [Required]
        public Guid OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public User Owner { get; set; }

        public string Location { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public EventCategory Category { get; set; }

        public List<Ticket> Tickets { get; set; } = new();
        [Required]
        public bool IsCanceled { get; set; } = false;

    }

}
