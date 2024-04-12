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
    }
}

