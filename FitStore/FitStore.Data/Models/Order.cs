namespace FitStore.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Order
    {
        public int Id { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalPrice { get; set; }

        public DateTime PurchaseDate { get; set; }

        [Required]
        public string UserId { get; set; }

        public User User { get; set; }

        public IList<OrderSupplements> Supplements { get; set; } = new List<OrderSupplements>();
    }
}