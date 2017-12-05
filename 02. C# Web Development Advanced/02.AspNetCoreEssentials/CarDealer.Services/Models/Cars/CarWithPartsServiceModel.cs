namespace CarDealer.Services.Models.Cars
{
    using Parts;
    using System.Collections.Generic;

    public class CarWithPartsServiceModel : CarServiceModel
    {
        public IEnumerable<PartServiceModel> Parts { get; set; }
    }
}