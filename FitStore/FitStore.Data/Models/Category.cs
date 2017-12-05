namespace FitStore.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static DataConstants;

    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MinLength(CategoryNameMinLength)]
        [MaxLength(CategoryNameMaxLength)]
        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        public IList<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
    }
}