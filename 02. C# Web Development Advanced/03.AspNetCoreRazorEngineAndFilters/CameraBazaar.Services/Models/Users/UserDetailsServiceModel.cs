namespace CameraBazaar.Services.Models.Users
{
    using Services.Models.Cameras;
    using System;
    using System.Collections.Generic;

    public class UserDetailsServiceModel
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Cameras { get; set; }

        public DateTime LastLoginTime { get; set; }

        public IEnumerable<CameraListServiceModel> CamerasOwned { get; set; }
    }
}