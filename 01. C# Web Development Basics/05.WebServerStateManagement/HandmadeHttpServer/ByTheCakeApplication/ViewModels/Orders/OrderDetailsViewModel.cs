namespace HandmadeHttpServer.ByTheCakeApplication.ViewModels.Orders
{
    using System;
    using System.Collections.Generic;

    public class OrderDetailsViewModel
    {
        public int Id { get; set; }

        public IList<int> ProductIds { get; set; }

        public IList<string> ProductNames { get; set; }

        public IList<decimal> ProductPrices { get; set; }

        public IList<int> ProductQuantities { get; set; }

        public DateTime CreationDate { get; set; }
    }
}