using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EukairiaWeb.Data.Models
{
    public class LeaveRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; }

        [Required]
        public DateTime? StartDate { get; set; }

        [Required]
        public DateTime? EndDate { get; set; }

        [Required]
        public LeaveType LeaveType { get; set; }

        public string Reason { get; set; }

        public LeaveRequestStatus IsPending { get; set; } = LeaveRequestStatus.Pending;

        public DateTime StartDateDt
        {
            get { return StartDate ?? default(DateTime); } // Coalesce operator to provide default value if null
        }
        public DateTime EndDateDt
        {
            get { return EndDate ?? default(DateTime); }
        }
    }
    public enum LeaveType
    {
        Paid, 
        UnPaid,
        Vacation
    }

    public enum LeaveRequestStatus
    {
        Pending,  
        Approved, 
        Denied    
    }
}
