using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace MeetupWebApp.Features.Events.Shared
{
    public class EventViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Title is Required")]
        [StringLength(maximumLength: 50)]
        public string Title { get; set; } = null!;

        [StringLength(4000)]
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
        public IBrowserFile ImageFile { get; set; }
        public string? ImageUrl { get; set; }

        public EventViewModel()
        {
            BeginDate = DateOnly.FromDateTime(DateTime.Now);
            EndDate = DateOnly.FromDateTime(DateTime.Now);
            BeginTime = TimeOnly.FromDateTime(DateTime.Now);
            EndTime = TimeOnly.FromDateTime(DateTime.Now);
            ImageUrl = "/image-placeholder.jpg";
        }

    }
}
