namespace PROG3.Models
{
    public class Claim
    {
        public int ClaimId { get; set; }
        public string LecturerId { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal TotalAmount => HourlyRate * HoursWorked;
        public string Status { get; set; } 
        public string DocumentUrl { get; set; } 
        public DateTime SubmissionDate { get; set; }
        public string LecturerName { get; set; }
    }
}
