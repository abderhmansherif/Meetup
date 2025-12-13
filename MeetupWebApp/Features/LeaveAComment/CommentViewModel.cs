using System.ComponentModel.DataAnnotations;

namespace MeetupWebApp.Features.LeaveAComment
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = null!;
        [Required]
        public string Username { get; set; } = null!;
        public DateTime PostedOn { get; set; }
        public int EventId { get; set; }
        public int UserId {  get; set; }
    }
}
