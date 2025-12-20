using System.ComponentModel.DataAnnotations.Schema;

namespace MeetupWebApp.Data.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public required string PaymentId {  get; set; }
        public int UserId { get; set; }
        public int RASVPId {  get; set; }
        public string PaymentStatus { get; set; } = "Pending";  
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("RASVPId")]
        public RSVP? RASVP {  get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
