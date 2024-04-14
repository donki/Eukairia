namespace EukairiaWeb.Data.Models
{
    public class LeaveRequestSummary
    {
        public int TotalVacationRequestedDays { get; set; }
        public int ApprovedVacationDays { get; set; }
        public int PendingVacationDays { get; set; }
        public int TotalPaidLeaveDays { get; set; }
        public int TotalUnpaidLeaveDays { get; set; }
    }

}
