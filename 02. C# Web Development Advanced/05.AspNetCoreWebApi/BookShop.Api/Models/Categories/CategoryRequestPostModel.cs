namespace BookShop.Api.Models.Categories
{
    using Common.Mapping;
    using Data.Models;
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants;

    public class CategoryRequestPostModel : IMapFrom<Category>
    {
        [Required]
        [MinLength(CategoryNameMinLength)]
        [MaxLength(CategoryNameMaxLength)]
        public string Name { get; set; }
    }
}