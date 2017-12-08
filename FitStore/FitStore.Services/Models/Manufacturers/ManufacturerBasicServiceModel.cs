namespace FitStore.Services.Models.Manufacturers
{
    using Common.Mapping;
    using Data.Models;

    public class ManufacturerBasicServiceModel : IMapFrom<Manufacturer>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }
    }
}