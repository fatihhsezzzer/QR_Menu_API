using Mazina_Backend.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mazina_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyUpdateController : ControllerBase
    {
        private readonly MyDbContext _context;

        public DailyUpdateController(MyDbContext context)
        {
            _context = context;
        }
        [HttpGet("updated")]
        public async Task<IActionResult> GetUpdatedProducts()
        {
            var updatedProducts = await _context.DailyProductsUpdated
                .Where(p => p.Date >= DateTime.UtcNow.AddDays(-30)) // Son 30 günü filtreler
                .OrderByDescending(p => p.Date)
                .ToListAsync();

            return Ok(updatedProducts);

        }

        [HttpGet("inserted")]
        public async Task<IActionResult> GetInsertedProducts()
        {
            var insertedProducts = await _context.DailyProductsInserted
              .Where(p => p.Date >= DateTime.UtcNow.AddDays(-30)) // Son 30 günü filtreler
              .OrderByDescending(p => p.Date)
              .ToListAsync();

            return Ok(insertedProducts);

        }
    }
}
