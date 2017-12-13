namespace FitStore.Services.Contracts
{
    using Models;
    using Models.Orders;
    using System.Threading.Tasks;

    public interface IOrderService
    {
        Task<OrderDetailsServiceModel> GetDetailsByIdAsync(int orderId);

        Task<bool> AddSupplementToCartAsync(int supplementId, ShoppingCart shoppingCart);

        Task<bool> RemoveSupplementFromCartAsync(int supplementId, ShoppingCart shoppingCart);

        Task<bool> RemoveAllSupplementsFromCartAsync(int supplementId, ShoppingCart shoppingCart);

        Task<bool> IsOrderExistingById(int orderId);

        Task CancelOrderAsync(ShoppingCart shoppingCart);

        Task FinishOrderAsync(string userId, ShoppingCart shoppingCart);
    }
}