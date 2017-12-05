namespace CarDealer.Web.Models.Suppliers
{
    using Services.Models.Suppliers;
    using System.Collections.Generic;

    public class SuppliersViewModel
    {
        public string Type { get; set; }

        public IEnumerable<SuppliersListServiceModel> Suppliers { get; set; }
    }
}