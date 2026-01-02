using System;
using System.ComponentModel.DataAnnotations;
namespace ElectronicCourses.Models
{
    public class Enrollment
    {
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    [Required]
    public int CourseId { get; set; }
    public Course? Course { get; set; }

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    [Range(0, 100)]
    public int ProgressPercent { get; set; } = 0;
    }
}