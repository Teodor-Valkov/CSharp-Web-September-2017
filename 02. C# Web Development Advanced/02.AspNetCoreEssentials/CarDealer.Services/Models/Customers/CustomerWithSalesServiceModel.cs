namespace CarDealer.Services.Models.Customers
{
    using Sales;
    using System.Collections.Generic;
    using System.Linq;

    public class CustomerWithSalesServiceModel
    {
        public string Name { get; set; }

        public bool IsYoungDriver { get; set; }

        public IEnumerable<SalePriceServiceModel> BoughtCars { get; set; }

        public decimal MoneySpent
        {
            get
            {
                return this.BoughtCars.Sum(c => c.Price * (1 - (decimal)c.Discount) * (this.IsYoungDriver ? 0.95m : 1));
            }
        }
    }
}