using EukairiaWeb.Data;
using EukairiaWeb.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EukairiaWeb.Services
{
    public class LeaveRequestService
    {
        private readonly AppDbContext _context;

        public LeaveRequestService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<LeaveRequest>> GetLeaveRequestsByUserAsync(Guid userId)
        {
            return await _context.LeaveRequests
                                 .Where(r => r.UserId == userId && r.IsPending != LeaveRequestStatus.Denied)
                                 .ToListAsync();
        }

        public async Task<List<LeaveRequest>> GetLeaveRequestsAsync()
        {
            return await _context.LeaveRequests.Include(lr => lr.User).ToListAsync();
        }

        public async Task AddLeaveRequestAsync(LeaveRequest request)
        {
            _context.LeaveRequests.Add(request);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLeaveRequestAsync(LeaveRequest request)
        {
            _context.LeaveRequests.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLeaveRequestAsync(LeaveRequest request)
        {
            _context.LeaveRequests.Remove(request);
            await _context.SaveChangesAsync();
        }

        public async Task<LeaveRequestSummary> CalculateLeaveRequestSummaryThisYearAsync(Guid? userId = null)
        {
            var query = _context.LeaveRequests.AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(lr => lr.UserId == userId.Value && lr.StartDate.Value.Year == DateTime.Now.Year);
            }

            var leaveRequests = await query.ToListAsync();

            var summary = new LeaveRequestSummary
            {
                TotalVacationRequestedDays = leaveRequests.Where(lr => lr.LeaveType == LeaveType.Vacation).Sum(lr => (lr.EndDate?.Date - lr.StartDate?.Date)?.Days + 1 ?? 0),
                ApprovedVacationDays = leaveRequests.Where(lr => lr.LeaveType == LeaveType.Vacation && lr.IsPending == LeaveRequestStatus.Approved).Sum(lr => (lr.EndDate?.Date - lr.StartDate?.Date)?.Days + 1 ?? 0),
                PendingVacationDays = leaveRequests.Where(lr => lr.LeaveType == LeaveType.Vacation && lr.IsPending == LeaveRequestStatus.Pending).Sum(lr => (lr.EndDate?.Date - lr.StartDate?.Date)?.Days + 1 ?? 0),
                TotalPaidLeaveDays = leaveRequests.Where(lr => lr.LeaveType == LeaveType.Paid).Sum(lr => (lr.EndDate?.Date - lr.StartDate?.Date)?.Days + 1 ?? 0),
                TotalUnpaidLeaveDays = leaveRequests.Where(lr => lr.LeaveType == LeaveType.UnPaid).Sum(lr => (lr.EndDate?.Date - lr.StartDate?.Date)?.Days + 1 ?? 0)
            };

            return summary;
        }


    }
}

