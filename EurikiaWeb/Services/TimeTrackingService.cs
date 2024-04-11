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



        public async Task<List<TimeTracking>> GetFilteredTimeTrackings(Guid UserId, DateTime? StartDate, DateTime? EndDate)
        {
            return await _context.TimeTrackings.Where(t => t.UserId.Equals(UserId) && t.Day >= StartDate && t.Day <= EndDate).ToListAsync();
        }
        public async Task<List<TimeTracking>> GetTimeTrackingsAsync()
        {
            return await _context.TimeTrackings.ToListAsync();
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

            // Busca un TimeTracking existente para el día con EndTime = null
            var existingTrack = await _context.TimeTrackings
                .Where(tt => tt.UserId == userId && tt.Day == startTime.Date && tt.EndTime == null)
                .FirstOrDefaultAsync();

            if (existingTrack != null)
            {
                // Si existe, actualiza el EndTime
                existingTrack.EndTime = endTime;
            }
            else
            {
                var newTrack = new TimeTracking
                {
                    UserId = userId,
                    Day = startTime.Date,
                    StartTime = startTime
                };

                _context.TimeTrackings.Add(newTrack);
            }

            await _context.SaveChangesAsync();
            await CalculateTimeTrackingAsync();
            return true;
        }


        public async Task Save(TimeTracking newTrack)
        {
            _context.TimeTrackings.Add(newTrack);
            await _context.SaveChangesAsync();
            await CalculateTimeTrackingAsync();
        }

        public async Task Update(TimeTracking newTrack)
        {
            _context.TimeTrackings.Update(newTrack);
            await _context.SaveChangesAsync();
            await CalculateTimeTrackingAsync();
        }

        public async Task<List<TimeTracking>> GetTodaysTracksAsync(Guid userId)
        {
            var today = DateTime.Today;
            return await _context.TimeTrackings
                .Where(t => t.UserId == userId && t.Day == today)
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



        public async Task CalculateTimeTrackingAsync()
        {
            var timeTrackings = _context.TimeTrackings.Where(t => !t.IsCalculated).ToList();

            foreach (var tracking in timeTrackings)
            {
                tracking.MinutesWithinShift = new TimeSpan();
                tracking.MinutesOutsideShift = new TimeSpan();
            }

                foreach (var tracking in timeTrackings)
            {
                var shifts = _context.WorkShifts.Where(s => s.UserId == tracking.UserId &&
                                                            s.StartDate <= tracking.Day &&
                                                            (s.EndDate == null || s.EndDate >= tracking.Day))
                                                 .ToList();

                foreach (var shift in shifts)
                {
                    if (shift.StartTime.HasValue && shift.EndTime.HasValue)
                    {
                        var shiftStartDateTime = tracking.Day.Date + shift.StartTime.Value;
                        var shiftEndDateTime = tracking.Day.Date + shift.EndTime.Value;

                        if (tracking.StartTime >= shiftStartDateTime && tracking.EndTime <= shiftEndDateTime  && tracking.EndTime!= null)
                        {
                            tracking.MinutesWithinShift = tracking.MinutesWithinShift.Add(tracking.EndTime.Value - tracking.StartTime);
                            tracking.IsCalculated = true;
                        }
                        else
                        {
                            tracking.MinutesOutsideShift = tracking.MinutesOutsideShift.Add(tracking.EndTime.Value - tracking.StartTime);
                            tracking.IsCalculated = true;
                        }
                    }
                }

                _context.TimeTrackings.Update(tracking);
            }

            await _context.SaveChangesAsync();
        }

        public TimeSpan GetWorkHours(Guid selectedUserId, DateTime? startDate, DateTime? endDate)
        {

            var res = new TimeSpan();
            var Hours = _context.TimeTrackings.Where(t => t.UserId.Equals(selectedUserId) && t.Day >= startDate && t.Day <= endDate).ToList();

            foreach (var h in Hours)
            {
                res = res.Add(h.MinutesWithinShift);
            }

            return res;
        }

        public TimeSpan GetOutSideHours(Guid selectedUserId, DateTime? startDate, DateTime? endDate)
        {

            var res = new TimeSpan();
            var Hours = _context.TimeTrackings.Where(t => t.UserId.Equals(selectedUserId) && t.Day >= startDate && t.Day <= endDate).ToList();

            foreach (var h in Hours)
            {
                res = res.Add(h.MinutesOutsideShift);
            }

            return res;
        }




    }

}
