using ems_backend.Models;
using ems_backend.Data;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class DiscussionService
{
    private readonly ApplicationDbContext _context;
    private readonly CloudinaryService _cloudinaryService;
    private readonly ILogger<DiscussionService> _logger;

    public DiscussionService(ApplicationDbContext context, CloudinaryService cloudinaryService, ILogger<DiscussionService> logger)
    {
        _context = context;
        _cloudinaryService = cloudinaryService;
        _logger = logger;
    }

    public async Task<DiscussionViewDto> CreateDiscussionAsync(NewDiscussionDto newDiscussion)
    {
        try
        {
            var eventExist = await _context.Events.FindAsync(newDiscussion.EventId);
            if (eventExist == null)
            {
                throw new Exception("Event not found");
            }

            var userExist = await _context.Users.FindAsync(newDiscussion.UserId);
            if (userExist == null)
            {
                throw new Exception("User not found");
            }

            List<string> photoUrls = new List<string>();

            if (newDiscussion.Photos != null && newDiscussion.Photos.Count > 0)
            {
                if (newDiscussion.Photos.Count > 5)
                {
                    throw new Exception("You can upload a maximum of 5 photos.");
                }

                photoUrls = await _cloudinaryService.UploadImagesAsync(newDiscussion.Photos);
            }

            var discussion = new Discussion
            {
                Id = Guid.NewGuid(),
                EventId = newDiscussion.EventId,
                UserId = newDiscussion.UserId,
                Title = newDiscussion.Title,
                Description = newDiscussion.Description,
                Photos = photoUrls.Count > 0 ? string.Join(",", photoUrls) : null,
                CreatedAt = DateTime.UtcNow.AddHours(7)
            };

            _context.Discussions.Add(discussion);
            await _context.SaveChangesAsync();

            // Log discussion creation
            _logger.LogInformation("New discussion added: {DiscussionId} for EventId {EventId} by UserId {UserId}",
                discussion.Id, discussion.EventId, discussion.UserId);

            return new DiscussionViewDto
            {
                Id = discussion.Id,
                EventId = discussion.EventId,
                UserId = discussion.UserId,
                UserName = discussion.User.FirstName + " " + discussion.User.LastName,
                Title = discussion.Title,
                Description = discussion.Description,
                Photos = discussion.Photos,
                CreatedAt = discussion.CreatedAt,
                UpdatedAt = discussion.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("Error creating discussion: {ErrorMessage}", ex.Message);
            throw;
        }
    }


    // Get all discussions by EventId
    public async Task<List<DiscussionViewDto>> GetDiscussionsByEventIdAsync(Guid eventId)
    {
        var discussions = await _context.Discussions
            .Where(d => d.EventId == eventId)
            .Include(d => d.User)
            .OrderByDescending(d => d.CreatedAt) // Order by recent dates
            .ToListAsync();

        return discussions.Select(d => new DiscussionViewDto
        {
            Id = d.Id,
            EventId = d.EventId,
            UserId = d.UserId,
            UserName = d.User.FirstName + " " + d.User.LastName,
            Title = d.Title,
            Description = d.Description,
            Photos = d.Photos,
            CreatedAt = d.CreatedAt,
            UpdatedAt = d.UpdatedAt
        }).ToList();
    }

    // Get a discussion by its Id
    public async Task<DiscussionViewDto> GetDiscussionByIdAsync(Guid discussionId)
    {
        var discussion = await _context.Discussions
            .FirstOrDefaultAsync(d => d.Id == discussionId);

        if (discussion == null)
        {
            throw new Exception("Discussion not found");
        }

        return new DiscussionViewDto
        {
            Id = discussion.Id,
            EventId = discussion.EventId,
            UserId = discussion.UserId,
            UserName = discussion.User.FirstName + " " + discussion.User.LastName,
            Title = discussion.Title,
            Description = discussion.Description,
            Photos = discussion.Photos,
            CreatedAt = discussion.CreatedAt,
            UpdatedAt = discussion.UpdatedAt
        };
    }

    // Update an existing discussion
    public async Task<DiscussionViewDto> UpdateDiscussionAsync(Guid Id, UpdateDiscussionDto updatedDiscussion)
    {
        var discussion = await _context.Discussions
            .FirstOrDefaultAsync(d => d.Id == Id);

        if (discussion == null)
        {
            throw new Exception("Discussion not found");
        }

        // Update title and description
        discussion.Title = updatedDiscussion.Title;
        discussion.Description = updatedDiscussion.Description;

        // If there are photos, upload them and update the photo URLs
        if (updatedDiscussion.Photos != null && updatedDiscussion.Photos.Count > 0)
        {
            if (updatedDiscussion.Photos.Count > 5)
            {
                throw new Exception("You can upload a maximum of 5 photos.");
            }

            var photoUrls = await _cloudinaryService.UploadImagesAsync(updatedDiscussion.Photos);
            discussion.Photos = string.Join(",", photoUrls);
        }

        // Update the last modified date (optional)
        discussion.UpdatedAt = DateTime.UtcNow.AddHours(7);

        _context.Discussions.Update(discussion);
        await _context.SaveChangesAsync();

        return new DiscussionViewDto
        {
            Id = discussion.Id,
            EventId = discussion.EventId,
            UserId = discussion.UserId,
            UserName = discussion.User.FirstName + " " + discussion.User.LastName,
            Title = discussion.Title,
            Description = discussion.Description,
            Photos = discussion.Photos,
            CreatedAt = discussion.CreatedAt,
            UpdatedAt = discussion.UpdatedAt
        };
    }

    // Delete a discussion
    public async Task DeleteDiscussionAsync(Guid discussionId)
    {
        var discussion = await _context.Discussions
            .FirstOrDefaultAsync(d => d.Id == discussionId);

        if (discussion == null)
        {
            throw new Exception("Discussion not found");
        }

        _context.Discussions.Remove(discussion);
        await _context.SaveChangesAsync();
    }

    // DTO classes
    public class NewDiscussionDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public List<IFormFile>? Photos { get; set; }
    }

    public class UpdateDiscussionDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<IFormFile>? Photos { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class DiscussionViewDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Photos { get; set; }  // Comma-separated list of URLs
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
