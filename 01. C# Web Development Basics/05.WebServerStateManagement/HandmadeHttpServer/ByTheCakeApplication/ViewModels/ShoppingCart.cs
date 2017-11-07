namespace HandmadeHttpServer.ByTheCakeApplication.Models
{
    using System.Collections.Generic;

    public class ShoppingCart
    {
        public const string CurrentShoppingCartSessionKey = "%^Current_Shopping_Cart^%";

        public IDictionary<int, int> ProductIdsWithQuantity { get; set; } = new Dictionary<int, int>();
    }
}