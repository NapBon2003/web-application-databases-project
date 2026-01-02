using ElectronicCourses.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ElectronicCourses.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Module> Modules => Set<Module>();
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    }
}
