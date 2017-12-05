namespace FitStore.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static DataConstants;

    public class Manufacturer
    {
        public int Id { get; set; }

        [Required]
        [MinLength(ManufacturerNameMinLength)]
        [MaxLength(ManufacturerNameMaxLength)]
        public string Name { get; set; }

        [Required]
        [MinLength(ManufacturerAddressMinLength)]
        [MaxLength(ManufacturerAddressMaxLength)]
        public string Address { get; set; }

        public bool IsDeleted { get; set; }

        public IList<Supplement> Supplements { get; set; } = new List<Supplement>();
    }
}