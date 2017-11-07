namespace ModPanel.App.Models.Admin
{
    using AutoMapper;
    using Data.Models;
    using Data.Models.Enums;
    using Infrastructure.Mapping;

    public class AdminUsersListModel : IMapFrom<User>, IHaveCustomMapping
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public PositionType Position { get; set; }

        public string Posts { get; set; }

        public bool IsApproved { get; set; }

        public void Configure(IMapperConfigurationExpression config)
        {
            config
               .CreateMap<User, AdminUsersListModel>()
               .ForMember(aulm => aulm.Posts, opt => opt.MapFrom(src => src.Posts.Count));
        }
    }
}