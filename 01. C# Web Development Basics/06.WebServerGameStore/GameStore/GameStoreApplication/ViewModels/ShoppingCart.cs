namespace GameStore.GameStoreApplication.ViewModels
{
    using System.Collections.Generic;

    public class ShoppingCart
    {
        public const string CurrentShoppingCartSessionKey = "%^Current_Shopping_Cart^%";

        public IList<int> GameIds { get; set; } = new List<int>();
    }
}