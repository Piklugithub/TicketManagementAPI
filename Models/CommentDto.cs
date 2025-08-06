namespace TicketManagementAPI.Models
{
    public class CommentDto
    {
        public int TicketId { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
    }
}
