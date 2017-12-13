namespace FitStore.Services.Models
{
    using Supplements;
    using System.Collections.Generic;

    public class ShoppingCart
    {
        public IList<SupplementInCartServiceModel> Supplements { get; set; } = new List<SupplementInCartServiceModel>();

        public decimal TotalPrice
        {
            get
            {
                decimal totalPrice = 0;

                foreach (SupplementInCartServiceModel supplement in this.Supplements)
                {
                    totalPrice += supplement.Price * supplement.Quantity;
                }

                return totalPrice;
            }
        }
    }
}