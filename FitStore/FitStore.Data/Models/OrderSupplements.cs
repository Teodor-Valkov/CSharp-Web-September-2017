﻿namespace FitStore.Data.Models
{
    public class OrderSupplements
    {
        public int OrderId { get; set; }

        public Order Order { get; set; }

        public int SupplementId { get; set; }

        public Supplement Supplement { get; set; }
    }
}