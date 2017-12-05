namespace BookShop.Services.Models.Categories
{
    using Common.Mapping;
    using Data.Models;
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants;

    public class CategoryDetailsServiceModel : IMapFrom<Category>
    {
        public int Id { get; set; }

        [Required]
        [MinLength(CategoryNameMinLength)]
        [MaxLength(CategoryNameMaxLength)]
        public string Name { get; set; }
    }
}