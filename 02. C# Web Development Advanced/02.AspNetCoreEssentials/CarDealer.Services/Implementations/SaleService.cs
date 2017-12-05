namespace CarDealer.Services.Implementations
{
    using Contracts;
    using Data;
    using Data.Models;
    using Models.Cars;
    using Models.Sales;
    using System.Linq;
    using System.Collections.Generic;

    public class SaleService : ISaleService
    {
        private CarDealerDbContext database;

        public SaleService(CarDealerDbContext database)
        {
            this.database = database;
        }

        public IEnumerable<SaleListServiceModel> GetAllListing(int page = 1, int pageSize = 10)
        {
            return this.database
                .Sales
                .OrderByDescending(s => s.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new SaleListServiceModel
                {
                    Id = s.Id,
                    CustomerName = s.Customer.Name,
                    IsYountDriver = s.Customer.IsYoungDriver,
                    Price = s.Car.Parts.Sum(p => p.Part.Price),
                    Discount = s.Discount,
                })
                .ToList();
        }

        public IEnumerable<SaleListServiceModel> GetAllDiscountedListing(double? discount, int page = 1, int pageSize = 10)
        {
            IQueryable<Sale> sales = this.database.Sales.AsQueryable();

            sales = discount != null
                ? sales.Where(s => s.Discount == discount / 100)
                : sales = sales.Where(s => s.Discount != 0);

            return sales
               .OrderByDescending(s => s.Id)
               .Skip((page - 1) * pageSize)
               .Take(pageSize)
               .Select(s => new SaleListServiceModel
               {
                   Id = s.Id,
                   CustomerName = s.Customer.Name,
                   IsYountDriver = s.Customer.IsYoungDriver,
                   Price = s.Car.Parts.Sum(p => p.Part.Price),
                   Discount = s.Discount,
               })
               .ToList();
        }

        public SaleDetailsServiceModel GetSaleById(int id)
        {
            return this.database
               .Sales
               .Where(s => s.Id == id)
               .Select(s => new SaleDetailsServiceModel
               {
                   Id = s.Id,
                   CustomerName = s.Customer.Name,
                   IsYountDriver = s.Customer.IsYoungDriver,
                   Price = s.Car.Parts.Sum(p => p.Part.Price),
                   Discount = s.Discount,
                   Car = new CarServiceModel
                   {
                       Id = s.Car.Id,
                       Make = s.Car.Make,
                       Model = s.Car.Model,
                       TravelledDistance = s.Car.TravelledDistance
                   }
               })
               .FirstOrDefault();
        }

        public decimal GetCarPrice(int id)
        {
            return this.database
                .Cars
                .Where(c => c.Id == id)
                .Sum(c => c.Parts.Sum(p => p.Part.Price));
        }

        public decimal GetCarPriceWithDiscount(int id, double discount, bool isYoungDriver)
        {
            return this.GetCarPrice(id) * (1 - (((decimal)discount / 100) + (isYoungDriver ? 0.05m : 0)));
        }

        public void Create(int customerId, int carId, double discount)
        {
            Sale sale = new Sale
            {
                CustomerId = customerId,
                CarId = carId,
                Discount = discount / 100
            };

            this.database.Sales.Add(sale);
            this.database.SaveChanges();
        }

        public int TotalSalesCount()
        {
            return this.database.Sales.Count();
        }

        public int TotalSalesWithDiscountCount(double? discount)
        {
            if (discount == null)
            {
                return this.database.Sales.Where(s => s.Discount != 0).Count();
            }

            return this.database.Sales.Where(s => s.Discount == discount / 100).Count();
        }
    }
}