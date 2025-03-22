using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ems_backend.Models
{
    public enum TicketStatus
    {
        NotRedeemed,
        Redeemed,
        Refunded
    }

    public class Ticket
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public Guid EventId { get; set; }

        [ForeignKey("EventId")]
        public Event Event { get; set; }

        [Required]
        public TicketStatus Status { get; set; } = TicketStatus.NotRedeemed;
    }

}
