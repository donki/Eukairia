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

        public int Minutes
        {
            get
            {
                // Si EndTime tiene valor, calcula la diferencia con StartTime
                if (EndTime != null)
                {
                    // Calcula la diferencia en minutos y retorna el valor
                    return (int)((EndTime.Value - StartTime).TotalMinutes);
                }
                else
                {
                    // Si no hay EndTime, podría retornar 0 o cualquier otro valor que indique "indefinido"
                    return 0;
                }
            }
        }
    }

}
