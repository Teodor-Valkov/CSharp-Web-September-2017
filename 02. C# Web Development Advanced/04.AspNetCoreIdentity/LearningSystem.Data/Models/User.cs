namespace LearningSystem.Data.Models
{
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static DataConstants;

    public class User : IdentityUser
    {
        [Required]
        [MinLength(UserNameMinLength)]
        [MaxLength(UserNameMaxLength)]
        public string Name { get; set; }

        public DateTime Birthdate { get; set; }

        public IList<Article> Articles { get; set; } = new List<Article>();

        public IList<Course> Trainings { get; set; } = new List<Course>();

        public IList<StudentCourse> Courses { get; set; } = new List<StudentCourse>();
    }
}