namespace _01.StudentSystem.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Student
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Phone { get; set; }

        public DateTime RegistrationDate { get; set; }

        public DateTime? Birthday { get; set; }

        public ICollection<Homework> Homeworks { get; set; } = new List<Homework>();

        public ICollection<StudentCourse> Courses { get; set; } = new List<StudentCourse>();
    }
}