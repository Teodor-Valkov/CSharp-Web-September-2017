namespace CarDealer.Web.Models.Customers
{
    using Services.Models.Customers;
    using Services.Models.Enums;
    using System.Collections.Generic;

    public class AllCustomersViewModel
    {
        public OrderDirection OrderDirection { get; set; }

        public IEnumerable<CustomerServiceModel> Customers { get; set; }
    }
}