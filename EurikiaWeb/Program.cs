using EukairiaWeb.Components;
using EukairiaWeb.Data;
using EukairiaWeb.Data.Models;
using EukairiaWeb.Helpers;
using EukairiaWeb.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;
using static System.Formats.Asn1.AsnWriter;

namespace EukairiaWeb
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>();


            builder.Services.AddScoped<UsersService>();

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;
                config.SnackbarConfiguration.PreventDuplicates = false;
                config.SnackbarConfiguration.NewestOnTop = true;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 5000;
                config.SnackbarConfiguration.HideTransitionDuration = 500;
                config.SnackbarConfiguration.ShowTransitionDuration = 500;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            await EnsureAdminRoleAndPermissions(app.Services);
            await EnsureUserRoleAndPermissions(app.Services);
            await EnsureAdminUser(app.Services);

            app.Run();
        }

        public static async Task EnsureAdminUser(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var adminEmail = "admin@eurikia.com";
            var adminUser = context.Users.ToList().Find(x => x.Email.Equals(adminEmail));
            if (adminUser == null)
            {
                var rolAdminUser = context.Roles.ToList().Find(x => x.RoleName.Equals("Administrador")); 
                adminUser = new User { Name = adminEmail, Email = adminEmail, Role = rolAdminUser };
                adminUser.Password = SecurityHelper.HashValue("AdministradorOC2024&");
                context.Users.Add(adminUser);
                await context.SaveChangesAsync();
            }
        }

        public static async Task EnsureAdminRoleAndPermissions(IServiceProvider serviceProvider)
        {
            var adminRoleName = "Administrador";
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
            var adminRoleName = "Usuario";
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var rolAdminUser = context.Roles.ToList().Find(x => x.RoleName.Equals(adminRoleName));
            if (rolAdminUser == null)
            {

                rolAdminUser = new Role { RoleName = adminRoleName };
                context.Roles.Add(rolAdminUser);
            }

            var permissions = new List<Permission>();
            permissions.Add(new Permission { Name = "Gestionar Usuarios", Description = "Permite crear, editar y eliminar usuarios", CanView = true, CanEdit = false });
            permissions.Add(new Permission { Name = "Gestionar Registro de Horas", Description = "Permite acceso total al registro de horas", CanView = true, CanEdit = false });

            rolAdminUser.Permissions = permissions;

            await context.SaveChangesAsync();

        }

    }



}

