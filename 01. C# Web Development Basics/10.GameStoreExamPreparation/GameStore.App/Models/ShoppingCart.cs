namespace GameStore.App.Models
{
    using System.Collections.Generic;

    public class ShoppingCart
    {
        public IList<int> GameIds { get; set; } = new List<int>();
    }
}