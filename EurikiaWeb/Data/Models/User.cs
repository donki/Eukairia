using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EukairiaWeb.Data.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UserId { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public Guid RoleId { get; set; }
        public Role Role { get; set; }

        public List<TimeTracking> TimeTrackings { get; set; }

    }

}
