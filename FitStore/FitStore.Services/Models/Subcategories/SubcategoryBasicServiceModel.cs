namespace FitStore.Services.Models.Subcategories
{
    using Common.Mapping;
    using Data.Models;

    public class SubcategoryBasicServiceModel : IMapFrom<Subcategory>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}