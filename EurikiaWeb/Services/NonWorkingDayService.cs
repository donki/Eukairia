using EukairiaWeb.Data;
using EukairiaWeb.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class NonWorkingDayService
{
    private readonly AppDbContext _context;

    public NonWorkingDayService(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddNonWorkingDayAsync(NonWorkingDay nonWorkingDay)
    {
        _context.NonWorkingDays.Add(nonWorkingDay);
        await _context.SaveChangesAsync();
    }

    public async Task<List<NonWorkingDay>> GetAllNonWorkingDaysAsync()
    {
        return await _context.NonWorkingDays.ToListAsync();
    }

    public async Task DeleteNonWorkingDayAsync(Guid id)
    {
        var day = await _context.NonWorkingDays.FindAsync(id);
        if (day != null)
        {
            _context.NonWorkingDays.Remove(day);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateNonWorkingDayAsync(NonWorkingDay nonWorkingDay)
    {
        _context.NonWorkingDays.Update(nonWorkingDay);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> Exists(DateTime date)
    {
        return await _context.NonWorkingDays.AnyAsync(nwd => nwd.Date.Date == date.Date);
    }

}
