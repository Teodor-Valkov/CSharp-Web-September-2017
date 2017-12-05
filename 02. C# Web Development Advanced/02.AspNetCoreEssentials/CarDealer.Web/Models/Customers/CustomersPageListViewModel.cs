namespace CarDealer.Web.Models.Customers
{
    using Services.Models.Customers;
    using Services.Models.Enums;
    using System.Collections.Generic;

    public class CustomersPageListViewModel
    {
        public OrderDirection OrderDirection { get; set; }

        public IEnumerable<CustomerServiceModel> Customers { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int PreviousPage
        {
            get
            {
                return this.CurrentPage == 1
                    ? 1
                    : this.CurrentPage - 1;
            }
        }

        public int NextPage
        {
            get
            {
                return this.CurrentPage == this.TotalPages
                    ? this.TotalPages
                    : this.CurrentPage + 1;
            }
        }
    }
}