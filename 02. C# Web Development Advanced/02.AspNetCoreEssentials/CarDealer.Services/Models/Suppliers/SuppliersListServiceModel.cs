namespace CarDealer.Services.Models.Suppliers
{
    public class SuppliersListServiceModel : SupplierServiceModel
    {
        public bool IsImporter { get; set; }

        public int TotalPartsCount { get; set; }
    }
}