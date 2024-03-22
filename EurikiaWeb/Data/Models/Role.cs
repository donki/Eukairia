using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security;
using Microsoft.AspNetCore.Identity;

namespace EukairiaWeb.Data.Models
{
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public ICollection<Permission> Permissions { get; set; }
        public ICollection<User> Users { get; set; }
    }

}
