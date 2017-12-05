namespace CarDealer.Web.Models.Cars
{
    using Services.Models.Cars;
    using System.Collections.Generic;

    public class CarsByMakeViewModel
    {
        public string Make { get; set; }

        public IEnumerable<CarServiceModel> Cars { get; set; }
    }
}