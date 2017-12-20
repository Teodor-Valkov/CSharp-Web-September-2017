namespace FitStore.Services.Models.Users
{
    using AutoMapper;
    using Common.Mapping;
    using Data.Models;
    using Models.Orders;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using static Common.CommonConstants;

    public class UserProfileServiceModel : IMapFrom<User>, IHaveCustomMapping
    {
        public string Username { get; set; }

        [Display(Name = UserFullNameName)]
        public string FullName { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        [Display(Name = UserPhoneNumberName)]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = UserBirthDateName)]
        public DateTime BirthDate { get; set; }

        public IEnumerable<OrderBasicServiceModel> Orders { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            string username = null;
            int page = default(int);

            mapper
              .CreateMap<User, UserProfileServiceModel>()
                .ForMember(dest => dest.Orders, opt => opt
                    .MapFrom(src => src.Orders
                        .Where(o => o.User.UserName == username)
                        .OrderByDescending(o => o.PurchaseDate)
                        .Skip((page - 1) * OrderPageSize)
                        .Take(OrderPageSize)));
        }
    }
}