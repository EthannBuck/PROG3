namespace PROG3.Models
{
    public class Invoice
    {
        public string ClaimId { get; set; }
        public string LecturerId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime DateIssued { get; set; }
        public string Status { get; set; }
    }
}
