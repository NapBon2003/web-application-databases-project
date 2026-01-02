using ElectronicCourses.Models;
using System.ComponentModel.DataAnnotations;

namespace ElectronicCourses.Models
{
    public class Module
    {
        public int Id { get; set; }

        [Required] 
        // title is required (wymagamy tytuł) 
        public string Title { get; set; } = default!;

        public string? Content { get; set; }
        public int Order { get; set; }

        public string? VideoPath { get; set; }

        [Required] // to jest klucz obcy, więc wymagany
        public int CourseId { get; set; }

        public virtual Course? Course { get; set; }
    }

}