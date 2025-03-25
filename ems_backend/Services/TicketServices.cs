using ems_backend.Models;
using ems_backend.Data;
using System;
using Microsoft.EntityFrameworkCore;

namespace ems_backend.Services
{
    public class TicketService
    {
        private readonly ApplicationDbContext _context;

        public TicketService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Book a ticket for an event
        public async Task<TicketDTO?> BookTicketAsync(Guid userId, Guid eventId)
        {
            var eventItem = await _context.Events.Include(e => e.Tickets).FirstOrDefaultAsync(e => e.Id == eventId);
            var user = await _context.Users.FindAsync(userId);

            if (eventItem == null || user == null || eventItem.TicketsLeft <= 0) return null;

            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                EventId = eventId,
                Status = TicketStatus.NotRedeemed
            };

            _context.Tickets.Add(ticket);
            eventItem.TicketsLeft--; // Reduce available tickets
            await _context.SaveChangesAsync();

            return new TicketDTO
            {
                Id = ticket.Id,
                EventId = ticket.EventId,
                EventName = eventItem.Name,
                EventStart = eventItem.StartDate,
                EventEnd = eventItem.EndDate,
                EventStatus = GetEventStatus(ticket),
                UserId = user.Id,
                UserFirstName = user.FirstName,
                UserLastName = user.LastName,
                UserEmail = user.Email,
                UserPhone = user.Phone,
                Status = ticket.Status
            };
        }


        // Get all ticket by User
        public async Task<List<TicketDTO>> GetTicketsByUserAsync(Guid userId)
        {
            return await _context.Tickets
                .Include(t => t.Event)
                .Include(t => t.User)
                .Where(t => t.UserId == userId)
                .Select(t => new TicketDTO
                {
                    Id = t.Id,
                    EventId = t.EventId,
                    EventName = t.Event.Name,
                    EventStart = t.Event.StartDate,
                    EventEnd = t.Event.EndDate,
                    EventStatus = GetEventStatus(t),
                    EventLocation = t.Event.Location,
                    UserId = t.UserId,
                    UserFirstName = t.User.FirstName,
                    UserLastName = t.User.LastName,
                    UserEmail = t.User.Email,
                    UserPhone = t.User.Phone,
                    Status = t.Status
                })
                .ToListAsync();
        }

        // Get all ticket by Event
        public async Task<List<TicketDTO>> GetTicketsByEventAsync(Guid eventId)
        {
            return await _context.Tickets
                .Include(t => t.User)
                .Include(t => t.Event)
                .Where(t => t.EventId == eventId)
                .Select(t => new TicketDTO
                {
                    Id = t.Id,
                    EventId = t.EventId,
                    EventName = t.Event.Name,
                    EventStart = t.Event.StartDate,
                    EventEnd = t.Event.EndDate,
                    EventStatus = GetEventStatus(t),
                    EventLocation = t.Event.Location,
                    UserId = t.UserId,
                    UserFirstName = t.User.FirstName,
                    UserLastName = t.User.LastName,
                    UserEmail = t.User.Email,
                    UserPhone = t.User.Phone,
                    Status = t.Status
                })
                .ToListAsync();
        }

        // Redeem a ticket
        public async Task<bool> RedeemTicketAsync(Guid ticketId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null || ticket.Status == TicketStatus.Redeemed) return false;

            ticket.Status = TicketStatus.Redeemed;
            await _context.SaveChangesAsync();
            return true;
        }

        // Get Event Status
        private static string GetEventStatus(Ticket t)
        {
            DateTime now = DateTime.UtcNow;

            if (t.Event.IsCanceled)
                return "Cancelled";

            if (now < t.Event.StartDate)
                return "Check-in Not Available";

            if (now >= t.Event.StartDate && now <= t.Event.EndDate)
                return "Check-in Available";

            if (now > t.Event.EndDate)
                return "Event Finished";

            return "Active";
        }

        // Get ticket by ID
        public async Task<TicketDTO?> GetTicketByIdAsync(Guid ticketId)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Event)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null) return null;

            return new TicketDTO
            {
                Id = ticket.Id,
                EventId = ticket.EventId,
                EventName = ticket.Event.Name,
                EventStart = ticket.Event.StartDate,
                EventEnd = ticket.Event.EndDate,
                EventStatus = GetEventStatus(ticket),
                EventLocation = ticket.Event.Location,
                UserId = ticket.UserId,
                UserFirstName = ticket.User.FirstName,
                UserLastName = ticket.User.LastName,
                UserEmail = ticket.User.Email,
                UserPhone = ticket.User.Phone,
                Status = ticket.Status
            };

        }

        // Get a ticket by User ID and Event ID
        public async Task<TicketDTO?> GetTicketByUserAndEventAsync(Guid userId, Guid eventId)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Event)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.UserId == userId && t.EventId == eventId);

            if (ticket == null) return null;

            return new TicketDTO
            {
                Id = ticket.Id,
                EventId = ticket.EventId,
                EventName = ticket.Event.Name,
                EventStart = ticket.Event.StartDate,
                EventEnd = ticket.Event.EndDate,
                EventStatus = GetEventStatus(ticket),
                EventLocation = ticket.Event.Location,
                UserId = ticket.UserId,
                UserFirstName = ticket.User.FirstName,
                UserLastName = ticket.User.LastName,
                UserEmail = ticket.User.Email,
                UserPhone = ticket.User.Phone,
                Status = ticket.Status
            };
        }

        // DTO
        public class TicketDTO
        {
            public Guid Id { get; set; }
            public Guid EventId { get; set; }
            public string EventName { get; set; }
            public DateTime EventStart { get; set; }
            public DateTime EventEnd { get; set; }
            public string EventStatus { get; set; }
            public string EventLocation { get; set; }
            public Guid UserId { get; set; }
            public string UserFirstName { get; set; }
            public string UserLastName { get; set; }
            public string UserEmail { get; set; }
            public string UserPhone { get; set; }
            public TicketStatus Status { get; set; }
        }
    }
}