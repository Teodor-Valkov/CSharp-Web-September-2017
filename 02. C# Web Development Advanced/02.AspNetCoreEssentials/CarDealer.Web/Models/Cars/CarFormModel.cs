namespace CarDealer.Web.Models.Cars
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CarFormModel
    {
        [Required]
        [MaxLength(50)]
        public string Make { get; set; }

        [Required]
        [MaxLength(50)]
        public string Model { get; set; }

        [Display(Name = "Travelled Distance")]
        [Range(0, long.MaxValue, ErrorMessage = "{0} must be positive number.")]
        public long TravelledDistance { get; set; }

        [Display(Name = "Parts")]
        public IEnumerable<int> PartsIds { get; set; }

        public IEnumerable<SelectListItem> AllParts { get; set; }
    }
}