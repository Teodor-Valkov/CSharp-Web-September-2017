namespace CameraBazaar.Services.Models.Identity
{
    using System.Collections.Generic;

    public class IdentityWithRolesBasicServiceModel
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}