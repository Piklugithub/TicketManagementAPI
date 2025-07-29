using System.ComponentModel.DataAnnotations;

namespace TicketManagementAPI.Models
{
    public class TicketGroup
    {
        [Key]
        public int GroupId { get; set; }

        [Required]
        [MaxLength(100)]
        public string GroupName { get; set; }

        // Navigation property
        public ICollection<GroupMember> Members { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
    }
}
