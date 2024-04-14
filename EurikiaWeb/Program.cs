using EukairiaWeb.Components;
using EukairiaWeb.Data;
using EukairiaWeb.Data.Models;
using EukairiaWeb.Helpers;
using EukairiaWeb.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using MudBlazor;
using MudBlazor.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Server;

namespace EukairiaWeb
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
                .EnableTokenAcquisitionToCallDownstreamApi()
                .AddInMemoryTokenCaches();

            builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();

            builder.Services.AddDbContext<AppDbContext>();


            builder.Services.AddScoped<UsersService>();
            builder.Services.AddScoped<TimeTrackingService>();
            builder.Services.AddScoped<WorkShiftService>();
            builder.Services.AddScoped<RolesService>();
            builder.Services.AddScoped<LeaveRequestService>();
            builder.Services.AddScoped<GlobalService>();
            builder.Services.AddScoped<NonWorkingDayService>();          



            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.Limits.MaxRequestBodySize = 10 * 1024 * 1024;
            });


            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
                //.AddInteractiveWebAssemblyComponents();

            builder.Services.AddMudMarkdownServices();

            builder.Services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;
                config.SnackbarConfiguration.PreventDuplicates = false;
                config.SnackbarConfiguration.NewestOnTop = true;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.BackgroundBlurred = true;
                config.SnackbarConfiguration.VisibleStateDuration = 5000;
                config.SnackbarConfiguration.HideTransitionDuration = 500;
                config.SnackbarConfiguration.ShowTransitionDuration = 500;
            });


            var app = builder.Build();


            app.UseAuthentication();
            app.UseAntiforgery();
            app.UseAuthorization();
            app.UseStatusCodePagesWithRedirects("/error");

            if (!app.Environment.IsDevelopment())
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.MapControllers();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();
                //.AddInteractiveWebAssemblyRenderMode();

            await EnsureAdminRoleAndPermissions(app.Services);
            await EnsureUserRoleAndPermissions(app.Services);
            await EnsureAdminUser(app.Services);

            app.Run();
        }

        public static async Task EnsureAdminUser(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var adminEmail = "admin@eukairia.com";
            var adminUser = context.Users.ToList().Find(x => x.Email.Equals(adminEmail));
            if (adminUser == null)
            {
                var rolAdminUser = context.Roles.ToList().Find(x => x.RoleName.Equals(RolesService.AdministratorRolName)); 
                adminUser = new User { Name = adminEmail, Email = adminEmail, Role = rolAdminUser };
                adminUser.Password = SecurityHelper.HashValue("AdministradorOC2024&");
                context.Users.Add(adminUser);
                await context.SaveChangesAsync();
            }
        }

        public static async Task EnsureAdminRoleAndPermissions(IServiceProvider serviceProvider)
        {
            var adminRoleName = RolesService.AdministratorRolName;
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var rolAdminUser = context.Roles.ToList().Find(x => x.RoleName.Equals(adminRoleName));
            if (rolAdminUser == null)
            {

                rolAdminUser = new Role { RoleName = adminRoleName};
                context.Roles.Add(rolAdminUser);
            }

            var permissions = new List<Permission>();
            permissions.Add(new Permission { Name = "Gestionar Usuarios", Description = "Permite crear, editar y eliminar usuarios", CanView = true, CanEdit = true });
            permissions.Add(new Permission { Name = "Gestionar Registro de Horas", Description = "Permite acceso total al registro de horas", CanView = true, CanEdit = true });

            rolAdminUser.Permissions = permissions;

            await context.SaveChangesAsync();

        }

        public static async Task EnsureUserRoleAndPermissions(IServiceProvider serviceProvider)
        {
            var RoleName = RolesService.UserRolName;
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var rolUser = context.Roles.ToList().Find(x => x.RoleName.Equals(RoleName));
            if (rolUser == null)
            {

                rolUser = new Role { RoleName = RoleName };
                context.Roles.Add(rolUser);
            }

            var permissions = new List<Permission>();
            permissions.Add(new Permission { Name = "Gestionar Usuarios", Description = "Permite crear, editar y eliminar usuarios", CanView = true, CanEdit = false });
            permissions.Add(new Permission { Name = "Gestionar Registro de Horas", Description = "Permite acceso total al registro de horas", CanView = true, CanEdit = false });

            rolUser.Permissions = permissions;

            await context.SaveChangesAsync();

        }


    }



}

