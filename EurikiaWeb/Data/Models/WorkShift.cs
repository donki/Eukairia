using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EukairiaWeb.Data.Models
{
    public class WorkShift
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid UserId { get; set; } // Clave foránea a User
        public TimeSpan? StartTime { get; set; } // Hora de inicio del turno
        public TimeSpan? EndTime { get; set; } // Hora de fin del turno
        public TimeSpan? MaxEntryTime { get; set; } // Hora máxima de entrada
        public TimeSpan? MinExitTime { get; set; } // Hora mínima de salida
        public TimeSpan? MaxHoursPerDay { get; set; } // Máximo de horas trabajadas por día
        public TimeSpan? HoursPerDay { get; set; } // Máximo de horas trabajadas por día
        public DaysOfWeek ActiveDays { get; set; } // Días de la semana activos para este turno


        // Propiedad de navegación
        [ForeignKey("UserId")]
        public User User { get; set; }

        public DateTime? StartDate { get; set; } // Fecha de inicio de aplicación del turno
        public DateTime? EndDate { get; set; } // Fecha final de aplicación del turno (opcional)

    }

    [Flags]
    public enum DaysOfWeek
    {
        Ninguno = 0,
        Lunes = 1,
        Martes = 2,
        Miercoles = 4,
        Jueves = 8,
        Viernes = 16,
        Sábado = 32,
        Domingo = 64
    }

}
