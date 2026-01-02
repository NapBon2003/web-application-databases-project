using Microsoft.AspNetCore.Identity;

namespace ElectronicCourses.Models
{
    public class ApplicationUser : IdentityUser
    {
    public string? FullName { get; set; }
    }
}