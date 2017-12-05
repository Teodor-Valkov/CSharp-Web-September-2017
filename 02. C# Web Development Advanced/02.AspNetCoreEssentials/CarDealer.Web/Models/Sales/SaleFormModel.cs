﻿namespace CarDealer.Web.Models.Sales
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class SaleFormModel
    {
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        public IEnumerable<SelectListItem> AllCustomers { get; set; }

        [Display(Name = "Car")]
        public int CarId { get; set; }

        public IEnumerable<SelectListItem> AllCars { get; set; }

        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100 percentages.")]
        public double Discount { get; set; }

        public IEnumerable<SelectListItem> AllDiscounts { get; set; }
    }
}