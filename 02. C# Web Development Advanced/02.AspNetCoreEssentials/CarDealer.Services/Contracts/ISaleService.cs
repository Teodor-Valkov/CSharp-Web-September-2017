namespace CarDealer.Services.Contracts
{
    using Models.Sales;
    using System.Collections.Generic;

    public interface ISaleService
    {
        IEnumerable<SaleListServiceModel> GetAllListing(int page, int pageSize);

        IEnumerable<SaleListServiceModel> GetAllDiscountedListing(double? discount, int page, int pageSize);

        SaleDetailsServiceModel GetSaleById(int id);

        decimal GetCarPrice(int id);

        decimal GetCarPriceWithDiscount(int id, double discount, bool isYoungDriver);

        void Create(int customerId, int carId, double discount);

        int TotalSalesCount();

        int TotalSalesWithDiscountCount(double? discount);
    }
}