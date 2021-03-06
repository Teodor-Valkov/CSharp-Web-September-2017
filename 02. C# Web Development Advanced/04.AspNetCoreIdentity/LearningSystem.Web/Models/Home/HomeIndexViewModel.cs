﻿namespace LearningSystem.Web.Models.Home
{
    using System.ComponentModel.DataAnnotations;

    public class HomeIndexViewModel
    {
        public string SearchToken { get; set; }

        [Display(Name = "Users")]
        public bool SearchInUsers { get; set; }

        [Display(Name = "Courses")]
        public bool SearchInCourses { get; set; }

        [Display(Name = "Articles")]
        public bool SearchInArticles { get; set; }
    }
}