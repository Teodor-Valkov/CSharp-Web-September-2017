namespace _05.ShopHierarchy.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Item
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public decimal Price { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        public ICollection<ItemOrder> Orders { get; set; } = new List<ItemOrder>();
    }
}