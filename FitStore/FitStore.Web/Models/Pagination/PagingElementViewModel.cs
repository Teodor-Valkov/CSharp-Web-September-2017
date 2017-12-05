namespace FitStore.Web.Models.Pagination
{
    using System.ComponentModel.DataAnnotations;

    public class PagingElementViewModel<TModel>
    {
        [Display(Name = "Search")]
        public string SearchToken { get; set; }

        public TModel Element { get; set; }

        public PaginationViewModel Pagination { get; set; }
    }
}