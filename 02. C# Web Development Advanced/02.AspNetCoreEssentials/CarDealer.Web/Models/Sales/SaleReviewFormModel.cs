namespace CarDealer.Web.Models.Sales
{
    using System.ComponentModel.DataAnnotations;

    public class SaleReviewFormModel
    {
        public int CustomerId { get; set; }

        [Display(Name = "Customer")]
        public string CustomerName { get; set; }

        public bool IsYoungDriver { get; set; }

        public int CarId { get; set; }

        [Display(Name = "Car")]
        public string CarMake { get; set; }

        public double Discount { get; set; }

        [Display(Name = "Car Price")]
        public decimal CarPrice { get; set; }

        [Display(Name = "Final Car Price")]
        public decimal FinalCarPrice { get; set; }
    }
}