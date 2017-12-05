namespace CarDealer.Services.Models.Sales
{
    using Cars;

    public class SaleDetailsServiceModel : SaleListServiceModel
    {
        public CarServiceModel Car { get; set; }
    }
}