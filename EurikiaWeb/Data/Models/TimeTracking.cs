namespace EukairiaWeb.Data.Models
{
    // TimeTracking.cs
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class TimeTracking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TimeTrackingId { get; set; }

        public Guid UserId { get; set; }
        public DateTime Day { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public int MinutesWithinShift { get; set; }

        public int MinutesOutsideShift { get; set; }
    }

}
