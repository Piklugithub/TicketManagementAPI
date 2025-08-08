using System.ComponentModel.DataAnnotations;

namespace TicketManagementAPI.Models
{
    public class CommentAttachment
    {
        [Key]
        public int AttachmentId { get; set; }

        public int CommentId { get; set; }

        public string FilePath { get; set; }
        public string? FileName { get; set; } // e.g. "image1.png", "document.pdf"

        public string FileType { get; set; } // e.g. image/png, application/pdf
        public DateTime UploadedAt { get; set; }

        public virtual TicketComment Comment { get; set; }
    }
}
