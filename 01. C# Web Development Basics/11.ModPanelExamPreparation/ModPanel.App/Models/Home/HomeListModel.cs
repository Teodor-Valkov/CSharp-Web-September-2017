namespace ModPanel.App.Models.Home
{
    using AutoMapper;
    using Data.Models;
    using Infrastructure.Mapping;
    using System;

    public class HomeListModel : IMapFrom<Post>, IHaveCustomMapping
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public void Configure(IMapperConfigurationExpression config)
        {
            config
             .CreateMap<Post, HomeListModel>()
             .ForMember(hlm => hlm.CreatedBy, opt => opt.MapFrom(src => src.User.Email));
        }
    }
}