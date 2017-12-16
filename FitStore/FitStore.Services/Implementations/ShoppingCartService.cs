namespace FitStore.Services.Implementations
{
    using Contracts;
    using Models;
    using Models.Supplements;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public class ShoppingCartService : IShoppingCartService
    {
        //private readonly ConcurrentDictionary<string, ShoppingCart> shoppingCars;

        //public ShoppingCartService()
        //{
        //    this.shoppingCars = new ConcurrentDictionary<string, ShoppingCart>();
        //}

        //public IList<SupplementInCartServiceModel> GetShoppingCartSupplements(string shoppingCartKey)
        //{
        //    ShoppingCart shoppingCart = this.GetShoppingCart(shoppingCartKey);

        //    IList<SupplementInCartServiceModel> supplementsInCart = shoppingCart.GetSupplementsInCart();

        //    return supplementsInCart;
        //}

        //public void AddOrUpdateSupplementToCart(string shoppingCartKey, SupplementInCartServiceModel supplementInCart)
        //{
        //    ShoppingCart shoppingCart = this.GetShoppingCart(shoppingCartKey);

        //    shoppingCart.AddSupplementToCart(supplementInCart);
        //}

        //public bool RemoveSupplementFromCart(string shoppingCartKey, SupplementInCartServiceModel supplementInCart)
        //{
        //    ShoppingCart shoppingCart = this.GetShoppingCart(shoppingCartKey);

        //    bool removeResult = shoppingCart.RemoveSupplementFromCart(supplementInCart);

        //    return removeResult;
        //}

        //public bool RemoveAllSupplementsFromCart(string shoppingCartKey, SupplementInCartServiceModel supplementInCart)
        //{
        //    ShoppingCart shoppingCart = this.GetShoppingCart(shoppingCartKey);

        //    bool removeAllResult = shoppingCart.RemoveAllSupplementsFromCart(supplementInCart);

        //    return removeAllResult;
        //}

        //public void ClearCart(string shoppingCartKey)
        //{
        //    ShoppingCart shoppingCart = this.GetShoppingCart(shoppingCartKey);

        //    shoppingCart.ClearCart();
        //}

        //private ShoppingCart GetShoppingCart(string shoppingCartKey)
        //{
        //    ShoppingCart shoppingCart = this.shoppingCars.GetOrAdd(shoppingCartKey, new ShoppingCart());

        //    return shoppingCart;
        //}
    }
}