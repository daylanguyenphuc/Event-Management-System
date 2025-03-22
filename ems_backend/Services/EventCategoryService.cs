using ems_backend.Data;
using ems_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace ems_backend.Services
{
    public class EventCategoryService
    {
        private readonly ApplicationDbContext _context;

        public EventCategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all event categories and return DTOs
        public async Task<List<EventCategoryDTO>> GetAllCategoriesAsync()
        {
            return await _context.EventCategories
                .Select(c => new EventCategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }
    }

    // DTO
    public class EventCategoryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
