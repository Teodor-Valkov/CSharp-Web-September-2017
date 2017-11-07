namespace GameStore.App.Infrastructure
{
    using Models;
    using WebServer.Http.Contracts;

    public static class HttpSessionExtensions
    {
        private const string CurrentShoppingCartSessionKey = "%^Current_Shopping_Cart^%";

        public static ShoppingCart GetShoppingCart(this IHttpSession session)
        {
            ShoppingCart shoppingCart = session.Get<ShoppingCart>(CurrentShoppingCartSessionKey);

            if (shoppingCart == null)
            {
                shoppingCart = new ShoppingCart();
                session.Add(CurrentShoppingCartSessionKey, shoppingCart);
            }

            return shoppingCart;
        }
    }
}