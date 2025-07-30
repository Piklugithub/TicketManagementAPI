using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TicketManagementAPI.Models;

namespace TicketManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public TicketsController(ApplicationDbContext context) => _context = context;

        [HttpGet("StatusCounts")]
        public async Task<IActionResult> GetTicketStatusCounts()
        {
            try
            {
                var statusCounts = await _context.Tickets
                    .GroupBy(t => t.Status)
                    .Select(g => new
                    {
                        Status = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                // Convert to dictionary for easy lookup
                var stats = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                {
                    { "Active", 0 },
                    { "Resolved", 0 },
                    { "On Hold", 0 },
                    { "Open", 0 }
                };

                foreach (var item in statusCounts)
                {
                    if (stats.ContainsKey(item.Status))
                        stats[item.Status] = item.Count;
                }

                return Ok(new
                {
                    success = true,
                    message = "Ticket status counts fetched successfully",
                    data = new
                    {
                        active = stats["Active"],
                        resolved = stats["Resolved"],
                        onHold = stats["On Hold"],
                        open = stats["Open"]
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while fetching ticket status counts",
                    error = ex.Message
                });
            }
        }
        [HttpGet("GetTicketsByStatus/{status}")]
        public async Task<IActionResult> GetTicketsByStatus(string status)
        {
            var tickets = await _context.Tickets
                .Where(t => t.Status == status)
                .Select(t => new {
                    t.TicketId,
                    t.Title,
                    t.Priority,
                    t.Status,
                    t.CreatedAt,
                    CreatedBy = t.CreatedByUser.FullName,
                    AssigmentTo = t.AssignedMember.MemberName,
                    Group = t.TicketGroup.GroupName
                })
                .ToListAsync();

            return Ok(new { success = true, data = tickets });
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateTicket([FromBody] Ticket ticket)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ticket.CreatedAt = DateTime.UtcNow;
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return Ok(ticket);
        }
        [HttpGet("groups")]
        public async Task<IActionResult> GetGroups()
        {
            var groups = await _context.TicketGroups.ToListAsync();
            return Ok(groups);
        }

        [HttpGet("group-members/{groupId}")]
        public async Task<IActionResult> GetGroupMembers(int groupId)
        {
            var members = await _context.GroupMembers
                .Where(m => m.GroupId == groupId)
                .ToListAsync();
            return Ok(members);
        }
    }
}
