namespace LearningSystem.Web.Models.Users
{
    using System.ComponentModel.DataAnnotations;

    public class PageViewModel<TModel>
    {
        [Display(Name = "Search")]
        public string SearchToken { get; set; }

        public TModel Element { get; set; }

        public PaginationViewModel Pagination { get; set; }
    }
}