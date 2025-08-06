using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketManagementAPI.Models;

namespace TicketManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketCommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public TicketCommentsController(ApplicationDbContext context) => _context = context;

        [HttpGet("{ticketId}")]
        public async Task<IActionResult> GetComments(int ticketId)
        {
            var comments = await _context.TicketComments
                .Where(c => c.TicketId == ticketId)
                .Include(c => c.User)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new {
                    c.Message,
                    c.CreatedAt,
                    c.User.UserId,
                    ProfilePicture = c.User.ProfilePicture != null ? Convert.ToBase64String(c.User.ProfilePicture) : null
                })
                .ToListAsync();

            return Ok(comments);
        }

        [HttpPost]
        public async Task<IActionResult> PostComment([FromBody] CommentDto comment)
        {
            var entity = new TicketComment
            {
                TicketId = comment.TicketId,
                UserId = comment.UserId,
                Message = comment.Message,
                CreatedAt = DateTime.UtcNow
            };

            _context.TicketComments.Add(entity);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
