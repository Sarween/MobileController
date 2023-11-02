using MobileController.Models;
using Microsoft.EntityFrameworkCore;

namespace MobileController.Models.Data
{
    public class MyDBContext : DbContext
    {
        public MyDBContext(DbContextOptions<MyDBContext> options) : base(options)
        {

        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Shift>().ToTable("Shift");
        //}

        public DbSet<Shift> Shift { get; set; }
        public DbSet<Recruitment> Recruitment { get; set; }

        public DbSet<Staff> Staff { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Parent> Parent { get; set; }
        public DbSet<Worksheet> Worksheet { get; set; }
    }
}