namespace Judge.App.Models.Contests
{
    using AutoMapper;
    using Data.Models;
    using Infrastructure.Mapping;

    public class ContestListModel : IMapFrom<Contest>, IHaveCustomMapping
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Submissions { get; set; }

        public int UserId { get; set; }

        public void Configure(IMapperConfigurationExpression config)
        {
            config
             .CreateMap<Contest, ContestListModel>()
             .ForMember(clm => clm.Submissions, opt => opt.MapFrom(src => src.Submissions.Count));
        }
    }
}