namespace FitStore.Services.Contracts
{
    using Models;
    using Models.Orders;
    using System.Threading.Tasks;

    public interface IOrderService
    {
        Task<OrderDetailsServiceModel> GetDetailsByIdAsync(int orderId);

        Task<bool> AddSupplementToCartAsync(int supplementId, ShoppingCart shoppingCart);

        bool RemoveSupplementFromCartAsync(int supplementId, ShoppingCart shoppingCart);

        bool RemoveAllSupplementsFromCartAsync(int supplementId, ShoppingCart shoppingCart);

        Task<bool> IsOrderExistingById(int orderId);

        Task<bool> IsLastAvailableSupplementAlreadyAdded(int supplementId, ShoppingCart shoppingCart);

        Task<bool> FinishOrderAsync(string userId, ShoppingCart shoppingCart);
    }
}