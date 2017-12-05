namespace CarDealer.Services.Implementations
{
    using Contracts;
    using Data;
    using Data.Models;
    using Models.Customers;
    using Models.Enums;
    using Models.Sales;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CustomerService : ICustomerService
    {
        private readonly CarDealerDbContext database;

        public CustomerService(CarDealerDbContext database)
        {
            this.database = database;
        }

        public IEnumerable<CustomerServiceModel> GetOrderedCustomersListing(OrderDirection orderDirection, int page, int pageSize)
        {
            IQueryable<Customer> customers = this.database
                .Customers
                .AsQueryable();

            switch (orderDirection)
            {
                case OrderDirection.Ascending:
                    customers = customers.OrderBy(c => c.BirthDate).ThenBy(c => !c.IsYoungDriver);
                    break;

                case OrderDirection.Descending:
                    customers = customers.OrderByDescending(c => c.BirthDate).ThenBy(c => !c.IsYoungDriver);
                    break;

                default:
                    throw new InvalidOperationException($"Invalid order direction: {orderDirection}");
            }

            return customers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CustomerServiceModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    BirthDate = c.BirthDate,
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToList();
        }

        public IEnumerable<CustomerBasicServiceModel> GetAllBasicCustomers()
        {
            return this.database
                .Customers
                .Select(c => new CustomerBasicServiceModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToList();
        }

        public CustomerWithSalesServiceModel GetCustomerWithSalesById(int id)
        {
            return this.database
                .Customers
                .Where(c => c.Id == id)
                .Select(c => new CustomerWithSalesServiceModel
                {
                    Name = c.Name,
                    IsYoungDriver = c.IsYoungDriver,
                    BoughtCars = c.Sales.Select(s => new SalePriceServiceModel
                    {
                        Price = s.Car.Parts.Sum(p => p.Part.Price),
                        Discount = s.Discount
                    }),
                })
                .FirstOrDefault();
        }

        public CustomerServiceModel GetCustomerById(int id)
        {
            return this.database
                .Customers
                .Where(c => c.Id == id)
                .Select(c => new CustomerServiceModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    BirthDate = c.BirthDate,
                    IsYoungDriver = c.IsYoungDriver
                })
                .FirstOrDefault();
        }

        public CustomerBasicServiceModel GetBasicCustomerById(int id)
        {
            return this.database
              .Customers
              .Where(c => c.Id == id)
              .Select(c => new CustomerBasicServiceModel
              {
                  Id = c.Id,
                  Name = c.Name,
                  IsYoungDriver = c.IsYoungDriver
              })
              .FirstOrDefault();
        }

        public void Create(string name, DateTime birthDate, bool isYoungDriver)
        {
            Customer customer = new Customer
            {
                Name = name,
                BirthDate = birthDate,
                IsYoungDriver = isYoungDriver
            };

            this.database.Customers.Add(customer);
            this.database.SaveChanges();
        }

        public void Edit(int id, string name, DateTime birthDate, bool isYoungDriver)
        {
            Customer customer = this.database.Customers.Find(id);

            if (customer == null)
            {
                return;
            }

            customer.Name = name;
            customer.BirthDate = birthDate;
            customer.IsYoungDriver = isYoungDriver;

            this.database.SaveChanges();
        }

        public bool IsCustomerExisting(int id)
        {
            return this.database.Customers.Any(c => c.Id == id);
        }

        public double TotalCustomersCount()
        {
            return this.database.Customers.Count();
        }
    }
}