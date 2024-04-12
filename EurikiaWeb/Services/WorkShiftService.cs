using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EukairiaWeb.Data.Models;
using EukairiaWeb.Data;
using Microsoft.EntityFrameworkCore;

public class WorkShiftService
{
    private readonly AppDbContext _context;

    public WorkShiftService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<WorkShift>> GetAllWorkShiftsAsync()
    {
        return await _context.WorkShifts.Include(ws => ws.User).ToListAsync();
    }

    public async Task<WorkShift> GetWorkShiftByIdAsync(Guid id)
    {
        return await _context.WorkShifts.Include(ws => ws.User)
            .FirstOrDefaultAsync(ws => ws.Id.Equals(id));
    }

    public async Task AddWorkShiftAsync(WorkShift workShift)
    {
        _context.WorkShifts.Add(workShift);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateWorkShiftAsync(WorkShift workShift)
    {
        _context.WorkShifts.Update(workShift);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteWorkShiftAsync(Guid id)
    {
        var workShift = await _context.WorkShifts.FindAsync(id);
        if (workShift != null)
        {
            _context.WorkShifts.Remove(workShift);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> CanRegisterTime(Guid userId, DateTime dateTime)
    {
        /*var userWorkShifts = await _context.WorkShifts
            .Where(ws => ws.UserId == userId &&
                         ws.StartDate <= dateTime &&
                         (ws.EndDate == null || ws.EndDate >= dateTime))
            .ToListAsync();

        if (!userWorkShifts.Any()) return false; 

        var dayOfWeek = dateTime.DayOfWeek;
        var timeOfDay = dateTime.TimeOfDay;

        foreach (var shift in userWorkShifts)
        {
            if (shift.ActiveDays.HasFlag(dayOfWeek) &&
                timeOfDay >= shift.StartTime &&
                timeOfDay <= shift.EndTime)
            {
                return true;
            }
        }

        return false;*/
        return true;
    }

    public async Task<TimeSpan> GetTheoricalHoursWorked(Guid userId, DateTime? startDate, DateTime? endDate)
    {
        var workShifts = await _context.WorkShifts
            .Where(w => w.UserId == userId &&
                        w.StartDate.Value.Date <= endDate &&
                        (w.EndDate.Value.Date >= startDate || w.EndDate == null))
            .ToListAsync();

        var totalHours = new TimeSpan();
        var DaysWithHoursPerDay = new List<DateTime>();

        foreach (var workShift in workShifts)
        {
            var day = workShift.StartDate.Value.Date;
            if (DaysWithHoursPerDay.IndexOf(day) == -1)
            {
                var shiftsForDay = workShifts.Where(ws => ws.StartDate.Value.Date == day);
                if (shiftsForDay.Any())
                {
                    var maxHoursShift = shiftsForDay.OrderByDescending(ws => ws.HoursPerDay.HasValue ? ws.HoursPerDay.Value : TimeSpan.Zero)
                                                    .FirstOrDefault();

                    if (maxHoursShift != null && maxHoursShift.HoursPerDay != null)
                    {
                        totalHours = totalHours.Add(maxHoursShift.HoursPerDay.Value);
                    }
                }
                DaysWithHoursPerDay.Add(day);
            }
        }

        var DateDiff = endDate.Value - startDate.Value;
        return TimeSpan.FromHours(totalHours.TotalHours * DateDiff.TotalDays);
    }

}


