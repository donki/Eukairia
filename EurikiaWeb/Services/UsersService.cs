using EukairiaWeb.Data.Models;
using EukairiaWeb.Data;
using Microsoft.EntityFrameworkCore;
using EukairiaWeb.Helpers;
using Microsoft.Graph;
using Azure.Identity;
using Microsoft.Graph.Models;
using User = EukairiaWeb.Data.Models.User;


namespace EukairiaWeb.Services
{
    public class UsersService
    {
        private readonly AppDbContext _context;
        private readonly RolesService _RolesService;

        public UsersService(AppDbContext context, RolesService rolesService)
        {
            _context = context;
            _RolesService = rolesService;
        }

        public async Task<List<User>> GetUsersAsync() => await _context.Users.ToListAsync();

        public User GetUserByEmail(string email)
        {
            return _context.Users.ToList().Find(x => x.Email.Equals(email));
        }

        public async Task<User> AddUserAsync(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
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


        public async Task ImportUsersFromAzureAd(string TenantId, string ClientId, string ClientSecret)
        {
            var scopes = new[] { "https://graph.microsoft.com/.default" };
            var clientSecretCredential = new ClientSecretCredential(TenantId, ClientId, ClientSecret);
            var graphServiceClient = new GraphServiceClient(clientSecretCredential, scopes);

            try
            {
                var ADusers = await graphServiceClient.Users.GetAsync();

                var ADUserList = ADusers.Value;

                while (ADusers.OdataNextLink != null)
                {
                    ADusers = await graphServiceClient.Users.WithUrl(ADusers.OdataNextLink).GetAsync();
                    ADUserList.AddRange(ADusers.Value);
                }


                foreach (var ADuser in ADUserList)
                {

                    var user = await _context.Users.FirstOrDefaultAsync(u => u.AzureAdGuid.Equals(Guid.Parse(ADuser.Id)));
                    if (user == null)
                    {
                        user = new User() { Name = ADuser.DisplayName, Email = ADuser.UserPrincipalName, AzureAdGuid = Guid.Parse(ADuser.Id), RoleId = await _RolesService.GetByUserRoleId(), Password = SecurityHelper.HashValue("EukairiaUser2012&") };
                        this.AddUserAsync(user);
                    }
                    else
                    {
                        user.AzureAdGuid = Guid.Parse(ADuser.Id);
                        user.Email = ADuser.UserPrincipalName;
                        user.Name = ADuser.DisplayName;
                        this.UpdateUserAsync(user);
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error recibiendo los usuarios desde Azure AD: {ex.Message}");
            }


        }
    }

}
