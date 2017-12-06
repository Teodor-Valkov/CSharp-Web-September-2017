namespace FitStore.Web.Models.Home
{
    using Services.Models.Categories;
    using Services.Models.Supplements;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class HomeIndexViewModel
    {
        public IEnumerable<CategoryAdvancedServiceModel> Categories { get; set; }

        public IEnumerable<SupplementAdvancedServiceModel> Supplements { get; set; }

        public string SearchToken { get; set; }

        [Display(Name = "Users")]
        public bool SearchInUsers { get; set; }

        [Display(Name = "Courses")]
        public bool SearchInCourses { get; set; }

        [Display(Name = "Articles")]
        public bool SearchInArticles { get; set; }
    }
}