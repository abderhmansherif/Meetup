using System.ComponentModel.DataAnnotations;

namespace MeetupWebApp.Data.Entities
{
    public class RSVP
    {
        public int Id {  get; set; }
        [Required]
        public int EventId {  get; set; }
        public Event? Event { get; set; }
        [Required]
        public int UserId { get; set; }
        public User? User {  get; set; }
        [Required]
        public DateTime RSVPDate {  get; set; }
        [Required]
        public string? Status { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PaymentId { get; set; }
        public string? RefundId { get; set; }
        public string? RefundStatus { get; set; }
        public List<Transaction>? Transactions { get; set; }

    }
}
