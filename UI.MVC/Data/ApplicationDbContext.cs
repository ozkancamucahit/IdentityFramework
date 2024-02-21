using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using UI.MVC.Models;

namespace UI.MVC.Data
{
    public sealed class ApplicationDbContext : IdentityDbContext<AppUser>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opt) : base(opt)
        {
            
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<IdentityUser>().ToTable("USERS").Property(p => p.Id).HasColumnName("UserId");
        //    modelBuilder.Entity<AppUser>().ToTable("USERS").Property(p => p.Id).HasColumnName("UserId");
        //    modelBuilder.Entity<IdentityRole>().ToTable("ROLES");
        //}

        public DbSet<AppUser> AppUser { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
                .Ignore(p => p.RoleId);

            builder.Entity<AppUser>()
                .Ignore(p => p.Role);

            builder.Entity<AppUser>()
                .Ignore(p => p.RoleList);
        }
    }
}
