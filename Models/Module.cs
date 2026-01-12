using ElectronicCourses.Models;
using System.ComponentModel.DataAnnotations;

namespace ElectronicCourses.Models
{
    public class Module
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tytuł modułu jest wymagany.")]
        [StringLength(85, ErrorMessage = "Tytuł może mieć maksymalnie 85 znaków.")]
        public string Title { get; set; } = default!;

        [Required(ErrorMessage = "Treść modułu jest wymagana.")]
        [StringLength(3000, ErrorMessage = "Treść może mieć maksymalnie 3000 znaków.")]
        public string? Content { get; set; }
        public int Order { get; set; }

        public string? VideoPath { get; set; }

        [Required] // to jest klucz obcy, więc wymagany
        public int CourseId { get; set; }

        public virtual Course? Course { get; set; }
    }

}