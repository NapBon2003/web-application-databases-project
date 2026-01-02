using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace ElectronicCourses.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public int Hours { get; set; }
        public string? CoverImagePath { get; set; }

        public virtual ICollection<Module> Modules { get; set; } = new List<Module>();
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }

}