namespace LearningSystem.Web.Areas.Blog.Models.Articles
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants;

    public class ArticleFormViewModel
    {
        [Required]
        [MinLength(ArticleTitleMinLength, ErrorMessage = "The {0} must be at least {1} symbols long.")]
        [MaxLength(ArticleTitleMaxLength, ErrorMessage = "The {0} must be less than {1} symbols long.")]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Publish Date")]
        public DateTime PublishDate { get; set; }
    }
}