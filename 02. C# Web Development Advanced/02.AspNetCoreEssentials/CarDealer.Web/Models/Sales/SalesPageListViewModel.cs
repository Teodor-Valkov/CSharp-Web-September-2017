namespace CarDealer.Web.Models.Sales
{
    using Services.Models.Sales;
    using System.Collections.Generic;

    public class SalesPageListViewModel
    {
        public IEnumerable<SaleListServiceModel> Sales { get; set; }

        public double? Discount { get; set; }

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