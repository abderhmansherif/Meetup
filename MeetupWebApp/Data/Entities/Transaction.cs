using System.ComponentModel.DataAnnotations.Schema;

namespace MeetupWebApp.Data.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public string? PaymentId { get; set; }
        public decimal Amount { get; set; }
        public int UserId { get; set; }
        public int RASVPId {  get; set; }
        public string PaymentType { get; set; } = string.Empty;
        public string? Status { get; set; } = string.Empty;  
        public DateTime PaymentAt { get; set; } = DateTime.Now;

        [ForeignKey("RASVPId")]
        public RSVP? RASVP {  get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
