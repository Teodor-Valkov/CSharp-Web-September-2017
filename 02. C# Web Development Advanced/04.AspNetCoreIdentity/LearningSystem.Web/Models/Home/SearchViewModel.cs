namespace LearningSystem.Web.Models.Home
{
    using Services.Blog.Models.Articles;
    using Services.Models.Courses;
    using Services.Models.Users;
    using System.Collections.Generic;

    public class SearchViewModel : HomeIndexViewModel
    {
        public IEnumerable<UserSearchServiceModel> Users { get; set; } = new List<UserSearchServiceModel>();

        public IEnumerable<CourseSearchServiceModel> Courses { get; set; } = new List<CourseSearchServiceModel>();

        public IEnumerable<ArticleSearchServiceModel> Articles { get; set; } = new List<ArticleSearchServiceModel>();
    }
}