namespace HandmadeHttpServer.ByTheCakeApplication.ViewModels.Orders
{
    using System;

    public class OrderListingViewModel
    {
        public int Id { get; set; }

        public DateTime CreationDate { get; set; }

        public decimal TotalSum { get; set; }
    }
}