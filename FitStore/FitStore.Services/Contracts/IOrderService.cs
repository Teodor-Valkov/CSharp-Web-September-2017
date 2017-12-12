namespace FitStore.Services.Contracts
{
    using Data.Models;
    using Models;
    using System.Threading.Tasks;

    public interface IOrderService
    {
        Task<bool> AddSupplementToCartAsync(int supplementId, ShoppingCart shoppingCart);

        Task<bool> RemoveSupplementFromCartAsync(int supplementId, ShoppingCart shoppingCart);

        Task<bool> RemoveAllSupplementsFromCartAsync(int supplementId, ShoppingCart shoppingCart);

        Task CancelOrderAsync(ShoppingCart shoppingCart);

        Task FinishOrderAsync(User user, ShoppingCart shoppingCart);
    }
}