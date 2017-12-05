namespace FitStore.Web.Areas.Manager.Models.Categories
{
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants;

    public class CategoryFormViewModel
    {
        [Required]
        [MinLength(CategoryNameMinLength)]
        [MaxLength(CategoryNameMaxLength)]
        public string Name { get; set; }
    }
}