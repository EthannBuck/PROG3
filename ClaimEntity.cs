using Microsoft.Azure.Cosmos.Table;

namespace PROG3.Models
{
    public class ClaimEntity : TableEntity
    {
        public string LecturerId { get; set; }
        public string ClaimId { get; set; }

        public double HourlyRate { get; set; }
        public double HoursWorked { get; set; }
        public double TotalAmount { get; set; }
        public string Status { get; set; }
        public string DocumentUrl { get; set; }
        public DateTime SubmissionDate { get; set; }
    }
}
