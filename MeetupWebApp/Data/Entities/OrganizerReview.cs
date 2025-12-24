using System.ComponentModel.DataAnnotations;

namespace MeetupWebApp.Data.Entities
{
    public class OrganizerReview
    {
        public int Id { get; set; }
        public string? Comment { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }
        public int OrganizerId { get; set; }
        public User? Organizer { get; set; }
        public DateTime ReviewDate { get; set; }
        public int ReviewUserId { get; set; }
        public User? ReviewerUser { get; set; }
    }
}
