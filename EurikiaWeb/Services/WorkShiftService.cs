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

    public async Task CreateWorkShiftAsync(WorkShift workShift)
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
        var userWorkShifts = await _context.WorkShifts
            .Where(ws => ws.UserId == userId &&
                         ws.StartDate <= dateTime &&
                         (ws.EndDate == null || ws.EndDate >= dateTime))
            .ToListAsync();

        // Verificar si el día y la hora actual caen dentro de algún turno válido
        if (!userWorkShifts.Any()) return false; // No hay turnos asignados

        var dayOfWeek = (DaysOfWeek)(1 << (int)dateTime.DayOfWeek);
        var timeOfDay = dateTime.TimeOfDay;

        foreach (var shift in userWorkShifts)
        {
            if ((shift.ActiveDays & dayOfWeek) != 0 && // El día actual está dentro de los días activos
                timeOfDay >= shift.StartTime &&
                timeOfDay <= shift.EndTime)
            {
                return true; // Encuentra un turno que coincide con el día y la hora actual
            }
        }

        return false; // No se encontró ningún turno que coincida con el día y la hora actual
    }

}


