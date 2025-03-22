using ems_backend.Services;
using Microsoft.AspNetCore.Mvc;
using ems_backend.Models;

namespace ems_backend.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class EventCategoryController : ControllerBase
    {
        private readonly EventCategoryService _categoryService;

        public EventCategoryController(EventCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }
    }
}
