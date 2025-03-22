using ems_backend.Models;
using ems_backend.Data;
using System;
using Microsoft.EntityFrameworkCore;

namespace ems_backend.Services
{
    public class EventService
    {
        private readonly ApplicationDbContext _context;

        public EventService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all events
        public async Task<List<EventDTO>> GetEventsAsync()
        {
            return await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Owner)
                .Include(e => e.Tickets)
                .Select(e => new EventDTO
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    TicketPrice = e.TicketPrice,
                    TicketsLeft = e.TicketsLeft,
                    OwnerId = e.OwnerId,
                    OwnerFirstName = e.Owner.FirstName,
                    OwnerLastName = e.Owner.LastName,
                    Location = e.Location,
                    CategoryId = e.CategoryId,
                    CategoryName = e.Category.Name,
                    TicketCount = e.Tickets.Count
                })
                .ToListAsync();
        }

        // Get events filter
        public async Task<List<EventDTO>> GetEventsFilterAsync(decimal? minPrice, decimal? maxPrice, Guid? ownerId, string? location, Guid? categoryId)
        {
            var query = _context.Events
                .Include(e => e.Category)
                .Include(e => e.Owner)
                .Include(e => e.Tickets)
                .AsQueryable();

            if (minPrice.HasValue)
                query = query.Where(e => e.TicketPrice >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(e => e.TicketPrice <= maxPrice.Value);

            if (ownerId.HasValue)
                query = query.Where(e => e.OwnerId == ownerId.Value);

            if (!string.IsNullOrEmpty(location))
                query = query.Where(e => e.Location.Contains(location));

            if (categoryId.HasValue)
                query = query.Where(e => e.CategoryId == categoryId.Value);

            return await query
                .Select(e => new EventDTO
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    TicketPrice = e.TicketPrice,
                    TicketsLeft = e.TicketsLeft,
                    OwnerId = e.OwnerId,
                    OwnerFirstName = e.Owner.FirstName,
                    OwnerLastName = e.Owner.LastName,
                    Location = e.Location,
                    CategoryId = e.CategoryId,
                    CategoryName = e.Category.Name,
                    TicketCount = e.Tickets.Count
                })
                .ToListAsync();
        }

        // Get event by ID
        public async Task<EventDTO?> GetEventByIdAsync(Guid id)
        {
            return await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Owner)
                .Include(e => e.Tickets)
                .Where(e => e.Id == id)
                .Select(e => new EventDTO
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    TicketPrice = e.TicketPrice,
                    TicketsLeft = e.TicketsLeft,
                    OwnerId = e.OwnerId,
                    OwnerFirstName = e.Owner.FirstName,
                    OwnerLastName = e.Owner.LastName,
                    Location = e.Location,
                    CategoryId = e.CategoryId,
                    CategoryName = e.Category.Name,
                    TicketCount = e.Tickets.Count
                })
                .FirstOrDefaultAsync();
        }

        // Create a new event
        public async Task<Event> CreateEventAsync(EventRequest newEventRequest)
        {
            // Convert EventRequest DTO to Event Model
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Name = newEventRequest.Name,
                Description = newEventRequest.Description,
                StartDate = newEventRequest.StartDate,
                EndDate = newEventRequest.EndDate,
                TicketPrice = newEventRequest.TicketPrice,
                TicketsLeft = newEventRequest.TicketsLeft,
                OwnerId = newEventRequest.OwnerId,
                Location = newEventRequest.Location,
                CategoryId = newEventRequest.CategoryId,
                IsCanceled = newEventRequest.IsCanceled
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();
            return newEvent;
        }

        // Update an existing event
        public async Task<Event?> UpdateEventAsync(int id, EventRequest updatedEvent)
        {
            var existingEvent = await _context.Events.FindAsync(id);
            if (existingEvent == null) return null;

            existingEvent.Name = updatedEvent.Name;
            existingEvent.Description = updatedEvent.Description;
            existingEvent.StartDate = updatedEvent.StartDate;
            existingEvent.EndDate = updatedEvent.EndDate;
            existingEvent.TicketPrice = updatedEvent.TicketPrice;
            existingEvent.TicketsLeft = updatedEvent.TicketsLeft;
            existingEvent.Location = updatedEvent.Location;

            await _context.SaveChangesAsync();
            return existingEvent;
        }

        // Cancel an event (mark as canceled and refund tickets)
        public async Task<bool> CancelEventAsync(Guid eventId)
        {
            var eventToCancel = await _context.Events.Include(e => e.Tickets).FirstOrDefaultAsync(e => e.Id == eventId);
            if (eventToCancel == null) return false;

            foreach (var ticket in eventToCancel.Tickets)
            {
                ticket.Status = TicketStatus.Refunded; // Simulating refund by updating status
            }

            eventToCancel.TicketsLeft = 0; // No more tickets available
            eventToCancel.IsCanceled = true; // Mark the event as canceled
            await _context.SaveChangesAsync();
            return true;
        }
    }

    //DTO
    public class EventRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TicketPrice { get; set; }
        public int TicketsLeft { get; set; }
        public Guid OwnerId { get; set; }
        public string Location { get; set; }
        public Guid CategoryId { get; set; }
        public bool IsCanceled { get; set; } = false;
    }

    public class EventDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TicketPrice { get; set; }
        public int TicketsLeft { get; set; }
        public Guid OwnerId { get; set; }
        public string OwnerFirstName { get; set; }
        public string OwnerLastName { get; set; }
        public string Location { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int TicketCount { get; set; }
    }
}