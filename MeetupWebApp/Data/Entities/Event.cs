using System.ComponentModel.DataAnnotations;

namespace MeetupWebApp.Data.Entities
{
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength: 50)]
        public string Title { get; set; } = null!;

        [StringLength(200)]
        public string? Description { get; set; }
        [Required]
        public DateOnly BeginDate { get; set; }
        [Required]
        public TimeOnly BeginTime { get; set; }
        [Required]
        public DateOnly EndDate { get; set; }
        [Required]
        public TimeOnly EndTime { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int Capacity { get; set; }
        public string? Location { get; set; }
        public string? EventLink { get; set; }

        [Required]
        public string ImageUrl { get; set; } =null!;
        public int OrganizerId { get; set; }
    }
}
