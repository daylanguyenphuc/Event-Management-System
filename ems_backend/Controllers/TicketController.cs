using ems_backend.Models;
using ems_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace ems_backend.Controllers
{
    [Route("api/tickets")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly TicketService _ticketService;

        public TicketController(TicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetTicketsByUser(Guid userId)
        {
            var tickets = await _ticketService.GetTicketsByUserAsync(userId);
            return Ok(tickets);
        }

        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetTicketsByEvent(Guid eventId)
        {
            var tickets = await _ticketService.GetTicketsByEventAsync(eventId);
            return Ok(tickets);
        }

        [HttpPost("book/{userId}/{eventId}")]
        public async Task<IActionResult> BookTicket(Guid userId, Guid eventId)
        {
            var ticket = await _ticketService.BookTicketAsync(userId, eventId);
            if (ticket == null) return BadRequest("Booking failed.");
            return Ok(ticket);
        }

        [HttpPost("redeem/{ticketId}")]
        public async Task<IActionResult> RedeemTicket(Guid ticketId)
        {
            var success = await _ticketService.RedeemTicketAsync(ticketId);
            if (!success) return BadRequest("Invalid ticket or already redeemed.");
            return Ok("Ticket redeemed.");
        }
    }

}

