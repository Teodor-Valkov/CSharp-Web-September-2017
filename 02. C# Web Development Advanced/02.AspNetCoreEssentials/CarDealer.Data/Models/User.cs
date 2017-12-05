namespace CarDealer.Data.Models
{
    using Microsoft.AspNetCore.Identity;
    using System.Collections.Generic;

    public class User : IdentityUser
    {
        public IList<Log> Logs { get; set; } = new List<Log>();
    }
}