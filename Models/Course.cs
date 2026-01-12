using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace ElectronicCourses.Models
{
    public class Course
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Tytuł jest wymagany.")]
        [StringLength(85, ErrorMessage = "Tytuł może mieć maksymalnie 85 znaków.")]
        public string Title { get; set; } = default!;
        [Required(ErrorMessage = "Opis jest wymagany.")]
        [StringLength(800, ErrorMessage = "Opis może mieć maksymalnie 800 znaków.")]
        public string? Description { get; set; }
        [Range(1, 250, ErrorMessage = "Liczba godzin musi być między 1 a 250.")]
        public int Hours { get; set; }
        public string? CoverImagePath { get; set; }

        public virtual ICollection<Module> Modules { get; set; } = new List<Module>();
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }

}