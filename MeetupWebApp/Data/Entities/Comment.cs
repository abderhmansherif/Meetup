using System.ComponentModel.DataAnnotations;

namespace MeetupWebApp.Data.Entities
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = null!;
        public DateTime PostedOn { get; set; } = DateTime.Now;
        public int EventId { get; set; }
        public Event? Event { get; set; }
    }
}
