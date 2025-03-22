using ems_backend.Data;
using ems_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace ems_backend.Services
{
    public class PaymentService
    {
        private readonly ApplicationDbContext _context;

        public PaymentService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Process a new payment (store payment info)
        public async Task<PaymentDTO?> ProcessPaymentAsync(PaymentRequest paymentRequest)
        {
            var user = await _context.Users.FindAsync(paymentRequest.UserId);
            var ticket = await _context.Tickets.FindAsync(paymentRequest.TicketId);

            if (user == null || ticket == null) return null;

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                UserId = paymentRequest.UserId,
                TicketId = paymentRequest.TicketId,
                CardInfo = paymentRequest.CardInfo, // Consider encrypting for security
                Status = PaymentStatus.Completed,
                Amount = paymentRequest.Amount,
                PaymentDate = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return new PaymentDTO
            {
                Id = payment.Id,
                UserId = payment.UserId,
                UserFirstName = user.FirstName,
                UserLastName = user.LastName,
                TicketId = payment.TicketId,
                Amount = payment.Amount,
                Status = payment.Status,
                PaymentDate = payment.PaymentDate
            };
        }

        // Get all payments for a user
        public async Task<List<PaymentDTO>> GetPaymentsByUserAsync(Guid userId)
        {
            return await _context.Payments
                .Include(p => p.Ticket)
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .Select(p => new PaymentDTO
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    UserFirstName = p.User.FirstName,
                    UserLastName = p.User.LastName,
                    TicketId = p.TicketId,
                    Amount = p.Amount,
                    Status = p.Status,
                    PaymentDate = p.PaymentDate
                })
                .ToListAsync();
        }

        // Get payment details by ID
        public async Task<PaymentDTO?> GetPaymentByIdAsync(Guid paymentId)
        {
            return await _context.Payments
                .Include(p => p.Ticket)
                .Include(p => p.User)
                .Where(p => p.Id == paymentId)
                .Select(p => new PaymentDTO
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    UserFirstName = p.User.FirstName,
                    UserLastName = p.User.LastName,
                    TicketId = p.TicketId,
                    Amount = p.Amount,
                    Status = p.Status,
                    PaymentDate = p.PaymentDate
                })
                .FirstOrDefaultAsync();
        }

        // Get payment by Ticket
        public async Task<PaymentDTO?> GetPaymentByTicketAsync(Guid ticketId)
        {
            return await _context.Payments
                .Include(p => p.Ticket)
                .Include(p => p.User)
                .Where(p => p.TicketId == ticketId)
                .Select(p => new PaymentDTO
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    UserFirstName = p.User.FirstName,
                    UserLastName = p.User.LastName,
                    TicketId = p.TicketId,
                    Amount = p.Amount,
                    Status = p.Status,
                    PaymentDate = p.PaymentDate
                })
                .FirstOrDefaultAsync();
        }
    }

    // DTO for Payment Request
    public class PaymentRequest
    {
        public Guid UserId { get; set; }
        public Guid TicketId { get; set; }
        public string CardInfo { get; set; } // Masked for security
        public decimal Amount { get; set; }
    }

    // DTO for Payment Response
    public class PaymentDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public Guid TicketId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
