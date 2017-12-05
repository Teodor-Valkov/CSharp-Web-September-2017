namespace CarDealer.Services.Contracts
{
    using Models.Customers;
    using Models.Enums;
    using System;
    using System.Collections.Generic;

    public interface ICustomerService
    {
        IEnumerable<CustomerServiceModel> GetOrderedCustomersListing(OrderDirection orderDirection, int page, int pageSize);

        IEnumerable<CustomerBasicServiceModel> GetAllBasicCustomers();

        CustomerWithSalesServiceModel GetCustomerWithSalesById(int id);

        CustomerServiceModel GetCustomerById(int id);

        CustomerBasicServiceModel GetBasicCustomerById(int id);

        bool IsCustomerExisting(int id);

        void Create(string name, DateTime birthDate, bool isYoungDriver);

        void Edit(int id, string name, DateTime birthDate, bool isYoungDriver);

        double TotalCustomersCount();
    }
}