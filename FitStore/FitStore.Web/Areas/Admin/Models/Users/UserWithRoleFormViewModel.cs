namespace FitStore.Web.Areas.Admin.Models.Users
{
    using System.ComponentModel.DataAnnotations;

    public class UserWithRoleFormViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Role { get; set; }
    }
}