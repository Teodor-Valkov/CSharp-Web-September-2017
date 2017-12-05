namespace CarDealer.Services.Models.Parts
{
    public class PartListServiceModel : PartServiceModel
    {
        public int Quantity { get; set; }

        public string SupplierName { get; set; }
    }
}