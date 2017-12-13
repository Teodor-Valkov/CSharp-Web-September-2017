namespace FitStore.Data.Models
{
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static DataConstants;

    public class User : IdentityUser
    {
        [MinLength(UserFullNameMinLength)]
        [MaxLength(UserFullNameMaxLength)]
        public string FullName { get; set; }

        [MinLength(UserAddressMinLength)]
        [MaxLength(UserAddressMaxLength)]
        public string Address { get; set; }

        public DateTime BirthDate { get; set; }

        public DateTime RegistrationDate { get; set; }

        public bool IsRestricted { get; set; }

        public IList<Order> Orders { get; set; } = new List<Order>();

        public IList<Review> Reviews { get; set; } = new List<Review>();

        public IList<Comment> Comments { get; set; } = new List<Comment>();
    }
}