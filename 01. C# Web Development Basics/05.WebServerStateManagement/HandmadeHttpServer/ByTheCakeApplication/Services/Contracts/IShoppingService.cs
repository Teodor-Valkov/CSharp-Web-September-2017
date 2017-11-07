namespace HandmadeHttpServer.ByTheCakeApplication.Services.Contracts
{
    using HandmadeHttpServer.ByTheCakeApplication.ViewModels.Orders;
    using System.Collections.Generic;

    public interface IShoppingService
    {
        void CreateOrder(int userId, IDictionary<int, int> productIdWithQuantity);

        IEnumerable<OrderListingViewModel> GetUserOrders(int userId);

        bool Exists(int orderId);

        OrderDetailsViewModel GetOrder(int orderId);
    }
}