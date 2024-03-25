namespace EukairiaWeb.Services
{
    // TimeTrackingService.cs
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using EukairiaWeb.Data.Models;
    using EukairiaWeb.Data;
    using Microsoft.EntityFrameworkCore;

    public class TimeTrackingService
    {
        private readonly AppDbContext _context;

        public TimeTrackingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> TryTrackTimeAsync(Guid userId, DateTime startTime, DateTime endTime)
        {
            var workShifts = await _context.WorkShifts
                .Where(ws => ws.UserId == userId &&
                             ws.StartDate <= startTime &&
                             (ws.EndDate == null || ws.EndDate >= startTime))
                .ToListAsync();

            var applicableShift = workShifts.FirstOrDefault(ws =>
                ws.ActiveDays.HasFlag((DaysOfWeek)(1 << (int)startTime.DayOfWeek)) &&
                ws.StartTime <= startTime.TimeOfDay &&
                (endTime == null || ws.EndTime >= endTime.TimeOfDay));

            if (applicableShift == null)
            {
                // No se encontró un turno aplicable. No se permite registrar el tiempo.
                return false;
            }

            var newTrack = new TimeTracking
            {
                UserId = userId,
                Day = startTime.Date,
                StartTime = startTime,
                EndTime = endTime,
                // Aquí podrías añadir más lógica para determinar si el tiempo está dentro o fuera del turno
                // y ajustar la entidad TimeTracking según sea necesario.
            };

            _context.TimeTrackings.Add(newTrack);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<TimeTracking>> GetTodaysTracksAsync()
        {
            var today = DateTime.Today;
            return await _context.TimeTrackings
                .Where(t => t.Day == today)
                .ToListAsync();
        }

        public async Task<TimeSpan> GetTimeTrackedToday(Guid userId)
        {
            var today = DateTime.Today;
            return await GetTimeTrackedForPeriod(userId, today, today);
        }

        public async Task<TimeSpan> GetTimeTrackedThisWeek(Guid userId)
        {
            var now = DateTime.Now;
            var startOfWeek = now.Date.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(6);
            return await GetTimeTrackedForPeriod(userId, startOfWeek, endOfWeek);
        }

        public async Task<TimeSpan> GetTimeTrackedThisMonth(Guid userId)
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
            return await GetTimeTrackedForPeriod(userId, startOfMonth, endOfMonth);
        }

        public async Task<TimeSpan> GetTimeTrackedThisYear(Guid userId)
        {
            var now = DateTime.Now;
            var startOfYear = new DateTime(now.Year, 1, 1);
            var endOfYear = new DateTime(now.Year, 12, 31);
            return await GetTimeTrackedForPeriod(userId, startOfYear, endOfYear);
        }

        private async Task<TimeSpan> GetTimeTrackedForPeriod(Guid userId, DateTime start, DateTime end)
        {
            var tracks = await _context.TimeTrackings
                .Where(t => t.UserId == userId && t.Day >= start && t.Day <= end && t.EndTime.HasValue)
                .ToListAsync();

            return tracks.Aggregate(TimeSpan.Zero, (total, next) => total.Add(next.EndTime.Value - next.StartTime));
        }

        public async Task<TimeTrackingSummary> GetTimeTrackingSummary(Guid userId)
        {
            var summary = new TimeTrackingSummary
            {
                Daily = await GetFormattedTimeTracked(await GetTimeTrackedToday(userId)),
                Weekly = await GetFormattedTimeTracked(await GetTimeTrackedThisWeek(userId)),
                Monthly = await GetFormattedTimeTracked(await GetTimeTrackedThisMonth(userId)),
                Yearly = await GetFormattedTimeTracked(await GetTimeTrackedThisYear(userId))
            };

            return summary;
        }

        private async Task<string> GetFormattedTimeTracked(TimeSpan timeSpan)
        {
            // Formatea la duración como HH:mm. Ajusta según tus necesidades.
            return $"{(int)timeSpan.TotalHours:00}:{timeSpan.Minutes:00}";
        }

        public async Task<(int minutesWithinShift, int minutesOutsideShift)> CalculateTime(Guid userId, DateTime StarTime, DateTime EndTime)
        {
            var workShifts = await _context.WorkShifts.Where(ws => ws.UserId == userId).ToListAsync();
            int minutesWithinShift = 0;
            int minutesOutsideShift = 0;

            if (EndTime == null)
            {
                return (minutesWithinShift, minutesOutsideShift);
            }

            var shift = workShifts.FirstOrDefault(ws => ws.StartTime <= StarTime.TimeOfDay && ws.EndTime >= (EndTime).TimeOfDay);

            if (shift != null)
            {
                minutesWithinShift += (int)((TimeSpan)(StarTime - EndTime)).TotalMinutes;

            }
            else
            {
                minutesOutsideShift += (int)((TimeSpan)(StarTime - EndTime)).TotalMinutes;


            }


            return (minutesWithinShift, minutesOutsideShift);
        }


    }

}
