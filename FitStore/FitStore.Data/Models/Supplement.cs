namespace FitStore.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static DataConstants;

    public class Supplement
    {
        public int Id { get; set; }

        [Required]
        [MinLength(SupplementNameMinLength)]
        [MaxLength(SupplementNameMaxLength)]
        public string Name { get; set; }

        [Required]
        [MinLength(SupplementDescriptionMinLength)]
        [MaxLength(SupplementDescriptionMaxLength)]
        public string Description { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public byte[] Picture { get; set; }

        public DateTime BestBeforeDate { get; set; }

        public bool IsDeleted { get; set; }

        public int SubcategoryId { get; set; }

        public Subcategory Subcategory { get; set; }

        public int ManufacturerId { get; set; }

        public Manufacturer Manufacturer { get; set; }

        public IList<Review> Reviews { get; set; } = new List<Review>();

        public IList<Comment> Comments { get; set; } = new List<Comment>();

        public IList<OrderSupplements> Orders { get; set; } = new List<OrderSupplements>();
    }
}