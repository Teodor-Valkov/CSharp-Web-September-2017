namespace CarDealer.Web.Models.Parts
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class PartFormModel : IValidatableObject
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be positive number.")]
        public decimal Price { get; set; }

        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }

        public IEnumerable<SelectListItem> Suppliers { get; set; }

        public bool IsEdit { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.Name == "Part")
            {
                yield return new ValidationResult("Name cannot be 'Part'.");
            }
        }
    }
}