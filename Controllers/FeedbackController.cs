using Mazina_Backend.Data;
using Mazina_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mazina_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly MyDbContext _context;
        public FeedbackController(MyDbContext context) => _context = context;

        [HttpPost]
        public async Task<IActionResult> SubmitFeedback([FromBody] FeedbackDto dto)
        {
            var feedback = new Feedback
            {
                Rating = dto.Rating,
                PhoneNumber = dto.PhoneNumber,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow // Server tarafında zaman belirleniyor
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Teşekkürler, geri bildiriminiz alındı." });
        }

        [HttpPost("track-google-review")]
        public async Task<IActionResult> TrackGoogleReviewClick()
        {
            var click = new GoogleReviewClick
            {
                ClickedAt = DateTime.UtcNow
            };

            _context.GoogleReviewClicks.Add(click);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("google-review-report")]
        public async Task<IActionResult> GetGoogleReviewStats([FromQuery] string period = "monthly")
        {
            var now = DateTime.UtcNow;
            DateTime fromDate = period switch
            {
                "weekly" => now.AddDays(-7),
                "monthly" => now.AddMonths(-1),
                _ => now.AddMonths(-1)
            };

            var count = await _context.GoogleReviewClicks
                .Where(c => c.ClickedAt >= fromDate)
                .CountAsync();

            return Ok(new { Period = period, Count = count });
        }


        // GET: api/Feedback
        [HttpGet]
        public IActionResult GetAllFeedback()
        {
            var feedbackList = _context.Feedbacks
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => new
                {
                    f.Id,
                    f.Rating,
                    f.PhoneNumber,
                    f.Comment,
                    f.CreatedAt
                })
                .ToList();

            return Ok(feedbackList);
        }
    }


public class FeedbackDto
    {
        public int Rating { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Comment { get; set; }
    }
}
