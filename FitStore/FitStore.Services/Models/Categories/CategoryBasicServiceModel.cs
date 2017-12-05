namespace FitStore.Services.Models.Categories
{
    using Common.Mapping;
    using Data.Models;

    public class CategoryBasicServiceModel : IMapFrom<Category>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}