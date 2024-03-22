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

        public async Task TrackTimeAsync(Guid userId)
        {
            var today = DateTime.Today;
            var now = DateTime.Now; // Captura la hora actual solo una vez para mantener consistencia
            now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);

            // Intenta encontrar un registro para el día actual, sin importar si tiene o no hora de finalización
            var existingTrack = await _context.TimeTrackings
                .Where(t => t.UserId == userId && t.Day == today)
                .OrderByDescending(t => t.StartTime)
                .FirstOrDefaultAsync();

            if (existingTrack != null)
            {
                // Si el registro existente ya tiene una hora de finalización, o si no la tiene...
                if (existingTrack.EndTime.HasValue)
                {
                    // ...y ya tiene EndTime, crea un nuevo registro, ya que el anterior ya fue cerrado
                    var newTrack = new TimeTracking
                    {
                        UserId = userId,
                        Day = today,
                        StartTime = now,
                        // EndTime se deja como null, indicando que este registro está abierto
                    };
                    _context.TimeTrackings.Add(newTrack);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Si no tiene EndTime, actualiza el registro existente para cerrarlo
                    existingTrack.EndTime = now;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                // Si no existe ningún registro para el día actual, crea uno nuevo
                var newTrack = new TimeTracking
                {
                    UserId = userId,
                    Day = today,
                    StartTime = now,
                    // EndTime se deja como null, indicando que este registro está abierto
                };
                _context.TimeTrackings.Add(newTrack);
                await _context.SaveChangesAsync();
            }
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
                .Where(t => t.UserId == userId && t.Day >= start && t.EndTime!=null && t.Day <= end && t.EndTime.HasValue)
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


    }

}
