namespace TicketManagementAPI.Models
{
    public class RegisterModel
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public IFormFile? ProfilePictureFile { get; set; }
        public byte[]? ProfilePicture { get; set; }
    }
}
