namespace CarDealer.Web.Models.Account
{
    using System.ComponentModel.DataAnnotations;

    public class ExternalLoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}