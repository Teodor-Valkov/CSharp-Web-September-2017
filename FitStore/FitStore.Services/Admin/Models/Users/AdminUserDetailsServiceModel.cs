namespace FitStore.Services.Admin.Models.Users
{
    using Common.Mapping;
    using Data.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static Common.CommonConstants;

    public class AdminUserDetailsServiceModel : IMapFrom<User>
    {
        public string Username { get; set; }

        [Display(Name = UserFullNameName)]
        public string FullName { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        [Display(Name = UserBirthDateName)]
        public DateTime BirthDate { get; set; }

        [Display(Name = UserRegistrationDateName)]
        public DateTime RegistrationDate { get; set; }

        [Display(Name = UserCurrentRolesName)]
        public IEnumerable<SelectListItem> CurrentRoles { get; set; }

        [Display(Name = UserAllRolesName)]
        public IEnumerable<SelectListItem> AllRoles { get; set; }
    }
}