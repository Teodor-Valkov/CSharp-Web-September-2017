namespace CarDealer.Web.Models.Suppliers
{
    using Services.Models.Suppliers;
    using System.Collections.Generic;

    public class SuppliersPageListViewModel
    {
        public IEnumerable<SuppliersListServiceModel> Suppliers { get; set; }

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