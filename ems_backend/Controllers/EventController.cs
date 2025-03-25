using ems_backend.Models;
using ems_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace ems_backend.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly EventService _eventService;

        public EventController(EventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _eventService.GetEventsAsync();
            return Ok(events);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetEventsFilter(
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] DateTime? StartDate,
            [FromQuery] DateTime? EndDate,
            [FromQuery] bool? AvailableOnly,
            [FromQuery] Guid? categoryId,
            [FromQuery] string? Name)
        {
            var events = await _eventService.GetEventsFilterAsync(minPrice, maxPrice, StartDate, EndDate, AvailableOnly, categoryId, Name);
            return Ok(events);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(Guid id)
        {
            var eventItem = await _eventService.GetEventByIdAsync(id);
            if (eventItem == null) return NotFound();
            return Ok(eventItem);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] EventRequest newEvent)
        {
            var createdEvent = await _eventService.CreateEventAsync(newEvent);
            return CreatedAtAction(nameof(GetEvent), new { id = createdEvent.Id }, createdEvent);
        }

        [HttpPost("cancel/{id}")]
        public async Task<IActionResult> CancelEvent(Guid id)
        {
            var result = await _eventService.CancelEventAsync(id);
            if (!result) return NotFound("Event not found.");
            return Ok("Event canceled.");
        }

        [HttpGet("owner/{ownerId}")]
        public async Task<IActionResult> GetEventsByOwnerId(Guid ownerId)
        {
            var events = await _eventService.GetEventsByOwnerIdAsync(ownerId);
            if (events == null || events.Count == 0)
            {
                return NotFound(new { message = "No events found for this owner." });
            }
            return Ok(events);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] EventRequest updatedEvent)
        {
            var updatedEventDTO = await _eventService.UpdateEventAsync(id, updatedEvent);

            if (updatedEventDTO == null)
            {
                return NotFound($"Event with id {id} not found.");
            }

            return Ok(updatedEventDTO);  // Return the updated event details as a DTO
        }
    }
}
