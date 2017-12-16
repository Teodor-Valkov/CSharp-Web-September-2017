namespace FitStore.Web.Models.Home
{
    using Services.Models.Categories;
    using Services.Models.Supplements;
    using System.Collections.Generic;

    public class HomeIndexViewModel
    {
        public IEnumerable<CategoryAdvancedServiceModel> Categories { get; set; }

        public IEnumerable<SupplementAdvancedServiceModel> Supplements { get; set; }
    }
}