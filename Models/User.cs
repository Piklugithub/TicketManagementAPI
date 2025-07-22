using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace TicketManagementAPI.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string? FullName { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public bool IsActive { get; set; } = true;
        public byte[]? ProfilePicture { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Ticket>? CreatedTickets { get; set; }
        public ICollection<TicketComment>? TicketComments { get; set; }
    }
}
