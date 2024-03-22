using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EukairiaWeb.Data.Models
{
    public class Permission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PermissionId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
        public List<Role> Roles { get; set; } = new List<Role>();
    }
}