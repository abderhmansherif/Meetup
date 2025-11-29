using System.ComponentModel.DataAnnotations;

namespace MeetupBlazorWebApp.Features.Events.CreateEvent
{
    public class EventViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Title is Required")]
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
        public int Capacity {  get; set; }
        public string? Location { get; set; }
        public string? EventLink {  get; set; }

        public DateTime GetBeginDateTime() => BeginDate.ToDateTime(BeginTime);
        public DateTime GetEndDateTime() => EndDate.ToDateTime(EndTime);

        public EventViewModel()
        {
            BeginDate = DateOnly.FromDateTime(DateTime.Now);
            EndDate = DateOnly.FromDateTime(DateTime.Now);
            BeginTime = TimeOnly.FromDateTime(DateTime.Now);
            EndTime = TimeOnly.FromDateTime(DateTime.Now);
        }

    }
}
