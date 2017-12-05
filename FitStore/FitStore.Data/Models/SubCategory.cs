namespace FitStore.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants;

    public class Subcategory
    {
        public int Id { get; set; }

        [Required]
        [MinLength(SubcategoryNameMinLength)]
        [MaxLength(SubcategoryNameMaxLength)]
        public string Name { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public bool IsDeleted { get; set; }

        public IList<Supplement> Supplements { get; set; } = new List<Supplement>();
    }
}