using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketManagementAPI.Models
{
    public class Ticket
    {
        [Key]
        public int TicketId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        public string Status { get; set; } = "Open";

        public string Priority { get; set; } = "Medium";

        // Foreign Key
        [ForeignKey("CreatedByUser")]
        public int CreatedBy { get; set; }

        public int? AssignedTo { get; set; }
        public int? GroupId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public User CreatedByUser { get; set; }
        [ForeignKey("AssignedTo")]
        public GroupMember AssignedMember { get; set; }

        [ForeignKey("GroupId")]
        public TicketGroup TicketGroup { get; set; }

        public ICollection<TicketComment> TicketComments { get; set; }
    }
}
