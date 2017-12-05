namespace CarDealer.Services.Models.Sales
{
    public class SaleListServiceModel : SalePriceServiceModel
    {
        public int Id { get; set; }

        public string CustomerName { get; set; }

        public bool IsYountDriver { get; set; }

        public decimal PriceWithDiscount
        {
            get
            {
                return this.Price * (1 - ((decimal)this.Discount + (this.IsYountDriver ? 0.05m : 0)));
            }
        }
    }
}