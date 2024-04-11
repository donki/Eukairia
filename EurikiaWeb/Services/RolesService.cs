using EukairiaWeb.Data;
using EukairiaWeb.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Solutions.VirtualEvents.Webinars.GetByUserIdAndRoleWithUserIdWithRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EukairiaWeb.Services
{


    public class RolesService
    {
        private readonly AppDbContext _context;

        public static readonly string AdministratorRolName = "Administrador";
        public static readonly string UserRolName = "Usuario";

        public RolesService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> GetByUserRoleId()
        {
            var res = await _context.Roles.FirstOrDefaultAsync(x => x.RoleName.Equals(UserRolName));
            return res.RoleId;
        }

        public async Task<List<Role>> GetRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Role> GetRoleByIdAsync(Guid roleId)
        {
            return await _context.Roles.FindAsync(roleId);
        }

        public async Task AddRoleAsync(Role role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRoleAsync(Role role)
        {
            _context.Entry(role).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRoleAsync(Guid roleId)
        {
            var role = await _context.Roles.FindAsync(roleId);
            if (role != null)
            {
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
            }
        }
    }

}
