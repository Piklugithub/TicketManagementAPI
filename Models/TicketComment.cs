using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketManagementAPI.Models
{
    public class TicketComment
    {
        [Key]
        public int CommentId { get; set; }
        public int TicketId { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }

        public Ticket Ticket { get; set; }
        public User User { get; set; }
        public virtual ICollection<CommentAttachment> Attachments { get; set; }
    }
}
