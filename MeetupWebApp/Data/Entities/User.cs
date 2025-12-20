using System.ComponentModel.DataAnnotations;

namespace MeetupWebApp.Data.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength: 200)]
        public string? Username { get; set; }

        [Required]
        [StringLength(maximumLength: 200)]
        public string? Email { get; set; }

        [Required]
        public string? UserRole { get; set; }

        public List<RSVP>? RSVPs { get; set; }
        public List<Comment>? Comments { get; set; }
        public List<Transaction>? Transactions { get; set; }

        // if the user is organizer
        public Event? Event { get; set; }
    }
}
