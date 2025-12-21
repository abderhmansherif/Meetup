using System.ComponentModel.DataAnnotations.Schema;

namespace MeetupWebApp.Data.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public string? PaymentId { get; set; }
        public string? RefundId {  get; set; }
        public int UserId { get; set; }
        public int RASVPId {  get; set; }
        public string? PaymentStatus { get; set; } = string.Empty;  
        public string? RefundStatus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("RASVPId")]
        public RSVP? RASVP {  get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
