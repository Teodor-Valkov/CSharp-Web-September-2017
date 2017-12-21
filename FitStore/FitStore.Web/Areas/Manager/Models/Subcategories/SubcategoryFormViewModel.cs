namespace FitStore.Web.Areas.Manager.Models.Subcategories
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static Common.CommonConstants;
    using static Common.CommonMessages;
    using static Data.DataConstants;

    public class SubcategoryFormViewModel
    {
        [Required]
        [StringLength(SubcategoryNameMaxLength, ErrorMessage = FieldLengthErrorMessage, MinimumLength = SubcategoryNameMinLength)]
        public string Name { get; set; }

        [Display(Name = CategoryEntity)]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}