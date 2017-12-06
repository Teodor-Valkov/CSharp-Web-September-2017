namespace FitStore.Web.Models.Pagination
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class PagingElementsViewModel<TModel>
    {
        [Display(Name = "Search")]
        public string SearchToken { get; set; }

        public bool IsDeleted { get; set; }

        public IEnumerable<TModel> Elements { get; set; }

        public PaginationViewModel Pagination { get; set; }
    }
}