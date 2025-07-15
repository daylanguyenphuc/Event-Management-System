using ems_backend.Models;
using ems_backend.Services;
using Microsoft.AspNetCore.Mvc;
using static DiscussionService;

namespace ems_backend.Controllers
{
    [Route("api/discussions")]
    [ApiController]
    public class DiscussionController : ControllerBase
    {
        private readonly DiscussionService _discussionService;

        public DiscussionController(DiscussionService discussionService)
        {
            _discussionService = discussionService;
        }

        [HttpGet("by-events/{eventId}/")]
        public async Task<IActionResult> GetDiscussionsByEventId(Guid eventId)
        {
            try
            {
                var discussions = await _discussionService.GetDiscussionsByEventIdAsync(eventId);
                if (discussions == null || !discussions.Any())
                {
                    return NotFound("No discussions found for this event.");
                }
                return Ok(discussions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiscussionById(Guid id)
        {
            try
            {
                var discussion = await _discussionService.GetDiscussionByIdAsync(id);
                return Ok(discussion);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);  // Return 404 with error message if not found
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDiscussion(Guid id, [FromForm] UpdateDiscussionDto model)
        {
            try
            {
                var updatedDiscussion = await _discussionService.UpdateDiscussionAsync(id, model);
                return Ok(updatedDiscussion);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);  // Return 404 if discussion is not found
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscussion(Guid id)
        {
            try
            {
                await _discussionService.DeleteDiscussionAsync(id);
                return NoContent();  // Return 204 No Content on successful delete
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);  // Return 404 if discussion is not found
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateDiscussion([FromForm] NewDiscussionDto newDiscussion)
        {
            try
            {
                var discussion = await _discussionService.CreateDiscussionAsync(newDiscussion);
                return CreatedAtAction(nameof(GetDiscussionById), new { id = discussion.Id }, discussion);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

