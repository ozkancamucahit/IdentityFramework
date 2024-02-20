using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using UI.MVC.Models;

namespace UI.MVC.Data
{
    public sealed class ApplicationDbContext : IdentityDbContext<IdentityUser>
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
    }
}
