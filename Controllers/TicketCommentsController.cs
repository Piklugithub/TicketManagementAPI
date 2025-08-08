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
        private readonly string _sharedFolder;
        public TicketCommentsController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _sharedFolder = config["AttachmentSettings:BasePath"];
        }
       

        [HttpGet("{ticketId}")]
        public async Task<IActionResult> GetComments(int ticketId)
        {
            try
            {
                var comments = await _context.TicketComments
                    .Where(c => c.TicketId == ticketId)
                    .Include(c => c.User)
                    .Include(c => c.Attachments)
                    .OrderBy(c => c.CreatedAt)
                    .Select(c => new
                    {
                        c.Message,
                        c.CreatedAt,
                        c.User.UserId,
                        userName = c.User.FullName,
                        ProfilePicture = c.User.ProfilePicture != null ? Convert.ToBase64String(c.User.ProfilePicture) : null,
                        Attachments = c.Attachments.Select(a => new
                        {
                            a.FilePath,
                            a.FileType,
                            a.FileName
                        }).ToList()
                    })
                    .ToListAsync();

                return Ok(comments);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [RequestSizeLimit(52428800)] // 50MB max file size
        public async Task<IActionResult> PostComment([FromForm] CommentDto commentDto, [FromForm] IFormFile file)
        {
            try
            {
                var comment = new TicketComment
                {
                    TicketId = commentDto.TicketId,
                    UserId = commentDto.UserId,
                    Message = commentDto.Message,
                    CreatedAt = DateTime.UtcNow
                };

                _context.TicketComments.Add(comment);
                await _context.SaveChangesAsync(); // Save first to get CommentId

                if (file != null && file.Length > 0)
                {
                    string ext = Path.GetExtension(file.FileName);
                    string orinalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                    string fileName = $"Ticket_{commentDto.TicketId}_{orinalFileName}{ext}";
                    string savePath = Path.Combine(_sharedFolder, fileName);

                    // Save file to local folder
                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Save only relative path for frontend access
                    var relativePath = Path.Combine("CommentAttachment", fileName).Replace("\\", "/");

                    var attachment = new CommentAttachment
                    {
                        CommentId = comment.CommentId,
                        FileName = fileName, // e.g. "Ticket_12345_image1.png"
                        FilePath = relativePath, // ✅ web-friendly path
                        FileType = ext,
                        UploadedAt = DateTime.UtcNow
                    };

                    _context.CommentAttachments.Add(attachment);
                    await _context.SaveChangesAsync();
                }

                return Ok(new { Success = true, comment.CommentId });
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
