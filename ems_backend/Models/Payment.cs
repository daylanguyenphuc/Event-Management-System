using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ems_backend.Models
{
    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed
    }

    public class Payment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string CardInfo { get; set; } // Masked for security

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public PaymentStatus Status { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")] // Ensures precision in the database
        public decimal Amount { get; set; }

        [Required]
        public Guid TicketId { get; set; }

        [ForeignKey("TicketId")]
        public Ticket Ticket { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow; // Auto-assign current time
    }
}
