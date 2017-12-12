﻿namespace FitStore.Services.Models.Supplements
{
    using AutoMapper;
    using Common.Extensions;
    using Common.Mapping;
    using Data.Models;
    using FitStore.Services.Models.Comments;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using static Common.CommonConstants;

    public class SupplementDetailsServiceModel : SupplementAdvancedServiceModel, IMapFrom<Supplement>, IHaveCustomMapping
    {
        public string Description { get; set; }

        public DateTime BestBeforeDate { get; set; }

        [Display(Name = CategoryEntity)]
        public string CategoryName { get; set; }

        [Display(Name = SubcategoryEntity)]
        public string SubcategoryName { get; set; }

        public IEnumerable<CommentAdvancedServiceModel> Comments { get; set; }

        public override void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<Supplement, SupplementDetailsServiceModel>()
                    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Subcategory.Category.Name))
                    .ForMember(dest => dest.SubcategoryName, opt => opt.MapFrom(src => src.Subcategory.Name))
                    .ForMember(dest => dest.Picture, opt => opt.MapFrom(src => src.Picture.FromByteArrayToString()))
                    .ForMember(dest => dest.ManufacturerName, opt => opt.MapFrom(src => src.Manufacturer.Name))
                    .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments.OrderByDescending(c => c.PublishDate)));
        }
    }
}