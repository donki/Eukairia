using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EukairiaWeb.Data.Models
{
    public class NonWorkingDay
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public NonWorkingDayType Type { get; set; }

        public string Description { get; set; }
    }

    public enum NonWorkingDayType
    {
        Weekend,
        Holiday,
    }
}
