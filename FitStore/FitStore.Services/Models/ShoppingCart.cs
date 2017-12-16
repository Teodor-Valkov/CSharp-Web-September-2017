namespace FitStore.Services.Models
{
    using Supplements;
    using System.Collections.Generic;
    using System.Linq;

    public class ShoppingCart
    {
        public IList<SupplementInCartServiceModel> Supplements { get; set; } = new List<SupplementInCartServiceModel>();

        //private readonly IList<SupplementInCartServiceModel> supplements;

        //public ShoppingCart()
        //{
        //    supplements = new List<SupplementInCartServiceModel>();
        //}

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

        //public IList<SupplementInCartServiceModel> GetSupplementsInCart()
        //{
        //    IList<SupplementInCartServiceModel> supplementsInCart = new List<SupplementInCartServiceModel>(this.supplements);

        //    return supplementsInCart;
        //}

        //public void AddSupplementToCart(SupplementInCartServiceModel supplementInCart)
        //{
        //    bool isSupplementAlreadyAddedToCart = this.supplements.Any(s => s.Id == supplementInCart.Id);

        //    if (isSupplementAlreadyAddedToCart)
        //    {
        //        supplementInCart = this.supplements
        //            .Where(s => s.Id == supplementInCart.Id)
        //            .First();

        //        supplementInCart.Quantity += 1;
        //    }
        //    else
        //    {
        //        supplementInCart.Quantity = 1;

        //        this.supplements.Add(supplementInCart);
        //    }
        //}

        //public bool RemoveSupplementFromCart(SupplementInCartServiceModel supplementInCart)
        //{
        //    bool isSupplementAddedToCart = this.supplements.Any(s => s.Id == supplementInCart.Id);

        //    if (isSupplementAddedToCart)
        //    {
        //        supplementInCart = this.supplements
        //            .Where(s => s.Id == supplementInCart.Id)
        //            .First();

        //        supplementInCart.Quantity -= 1;

        //        if (supplementInCart.Quantity == 0)
        //        {
        //            this.supplements.Remove(supplementInCart);
        //        }

        //        return true;
        //    }

        //    return false;
        //}

        //public bool RemoveAllSupplementsFromCart(SupplementInCartServiceModel supplementInCart)
        //{
        //    bool isSupplementAddedToCart = this.supplements.Any(s => s.Id == supplementInCart.Id);

        //    if (isSupplementAddedToCart)
        //    {
        //        supplementInCart = this.supplements
        //            .Where(s => s.Id == supplementInCart.Id)
        //            .FirstOrDefault();

        //        this.supplements.Remove(supplementInCart);

        //        return true;
        //    }

        //    return false;
        //}

        //public void ClearCart()
        //{
        //    this.supplements.Clear();
        //}
    }
}