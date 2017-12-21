namespace FitStore.Web.Areas.Manager.Models.Categories
{
    using System.ComponentModel.DataAnnotations;

    using static Common.CommonMessages;
    using static Data.DataConstants;

    public class CategoryFormViewModel
    {
        [Required]
        [StringLength(CategoryNameMaxLength, ErrorMessage = FieldLengthErrorMessage, MinimumLength = CategoryNameMinLength)]
        public string Name { get; set; }
    }
}