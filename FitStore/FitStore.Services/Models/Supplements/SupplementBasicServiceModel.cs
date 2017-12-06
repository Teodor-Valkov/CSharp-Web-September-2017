namespace FitStore.Services.Models.Supplements
{
    using Common.Mapping;
    using Data.Models;

    public class SupplementBasicServiceModel : IMapFrom<Supplement>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}