using EukairiaWeb.Data.Models;
using EukairiaWeb.Data;
using Microsoft.EntityFrameworkCore;
using EukairiaWeb.Helpers;

namespace EukairiaWeb.Services
{
    public class UsersService
    {
        private readonly AppDbContext _context;

        public UsersService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetUsersAsync() => await _context.Users.ToListAsync();

        public async Task<User> AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task DeleteUserAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public bool ValidateUser(string UserName, string Password)
        {
            var User = _context.Users.ToList().Find(x => x.Email.Equals(UserName));
            if (User == null)
            {
                return false;
            }
            else
            {
                if (SecurityHelper.HashValue(Password).Equals(User.Password))
                {
                    return true;
                }
                else 
                { 
                    return false; 
                }
            }
        }
    }

}
