namespace CameraBazaar.Services.Models.Identity
{
    public class IdentityBasicServiceModel
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public bool CanUserCreateCameras { get; set; }
    }
}