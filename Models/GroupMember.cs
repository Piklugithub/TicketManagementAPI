using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketManagementAPI.Models
{
    public class GroupMember
    {
        [Key]
        public int MemberId { get; set; }

        [Required]
        [MaxLength(100)]
        public string MemberName { get; set; }

        // Foreign key
        public int GroupId { get; set; }

        // Navigation properties
        [ForeignKey("GroupId")]
        public TicketGroup Group { get; set; }

        public ICollection<Ticket> AssignedTickets { get; set; }
    }
}
