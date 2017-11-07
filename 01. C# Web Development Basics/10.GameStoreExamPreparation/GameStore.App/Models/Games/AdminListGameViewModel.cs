namespace GameStore.App.Models.Games
{
    using AutoMapper;
    using Infrastructure.Mapping;
    using GameStore.Models;

    public class AdminListGameViewModel : IMapFrom<Game>, IHaveCustomMapping
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Size { get; set; }

        public decimal Price { get; set; }

        public void Configure(IMapperConfigurationExpression config)
        {
            config
              .CreateMap<Game, AdminListGameViewModel>()
              .ForMember(algvm => algvm.Name, opt => opt.MapFrom(src => src.Title));
        }
    }
}