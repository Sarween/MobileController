using MobileController.Models;
using Microsoft.EntityFrameworkCore;

namespace MobileController.Data
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
        public DbSet<Student> Student { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Recruitment>(entity =>
            {
                entity.Property(r => r.JobShiftDate)
                .HasConversion(
                    v => v.HasValue ? v.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                    v => v.HasValue ? DateOnly.FromDateTime(v.Value) : (DateOnly?)null);

                entity.Property(r => r.StartTime)
                .HasConversion(
                timeOnly => timeOnly.ToTimeSpan(),
                timeSpan => TimeOnly.FromTimeSpan(timeSpan));

                entity.Property(r => r.EndTime)
                .HasConversion(
                timeOnly => timeOnly.ToTimeSpan(),
                timeSpan => TimeOnly.FromTimeSpan(timeSpan));
            });
        }

    }
}