namespace FitStore.Services.Models.Supplements
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static Common.CommonConstants;

    public class SupplementCategoryServiceModel
    {
        [Display(Name = CategoryEntity)]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}