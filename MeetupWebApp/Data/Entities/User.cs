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
    }
}
