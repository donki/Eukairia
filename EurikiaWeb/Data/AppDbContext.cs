﻿using Microsoft.EntityFrameworkCore;
using EukairiaWeb.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace EukairiaWeb.Data
{


    public class AppDbContext : IdentityDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<TimeTracking> TimeTrackings { get; set; }

        public DbSet<WorkShift> WorkShifts { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; internal set; }

        public DbSet<NonWorkingDay> NonWorkingDays { get; internal set; }


        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite("Data Source=Eukairia.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>()
                .HasMany(r => r.Permissions)
                .WithMany(p => p.Roles)
                .UsingEntity(j => j.ToTable("RolePermissions"));

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.TimeTrackings)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId);
        }

    }

}
