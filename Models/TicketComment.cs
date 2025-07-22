using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketManagementAPI.Models
{
    public class TicketComment
    {
        [Key]
        public int CommentId { get; set; }

        [ForeignKey("Ticket")]
        public int TicketId { get; set; }

        [Required]
        public string CommentText { get; set; }

        [ForeignKey("CommentedByUser")]
        public int CommentedBy { get; set; }

        public DateTime CommentedAt { get; set; } = DateTime.Now;

        // Navigation
        public Ticket? Ticket { get; set; }
        public User? CommentedByUser { get; set; }
    }
}
