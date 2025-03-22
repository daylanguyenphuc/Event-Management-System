using ems_backend.Models;
using ems_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace ems_backend.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // Process a payment
        [HttpPost("process")]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
        {
            var payment = await _paymentService.ProcessPaymentAsync(request);
            return Ok(payment);
        }

        // Get all payments by user
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPaymentsByUser(Guid userId)
        {
            var payments = await _paymentService.GetPaymentsByUserAsync(userId);
            return Ok(payments);
        }

        // Get payment by ID
        [HttpGet("{paymentId}")]
        public async Task<IActionResult> GetPaymentById(Guid paymentId)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(paymentId);
            if (payment == null) return NotFound("Payment not found.");
            return Ok(payment);
        }

        // Get payment by Ticket
        [HttpGet("ticket/{ticketId}")]
        public async Task<IActionResult> GetPaymentByTicket(Guid ticketId)
        {
            var payment = await _paymentService.GetPaymentByTicketAsync(ticketId);
            if (payment == null) return NotFound("Payment not found for this ticket.");
            return Ok(payment);
        }

    }
}

