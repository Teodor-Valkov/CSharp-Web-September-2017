namespace CameraBazaar.Services.Models.Cameras
{
    using Data.Models.Enums;

    public class CameraListServiceModel
    {
        public int Id { get; set; }

        public Make Make { get; set; }

        public string Model { get; set; }

        public decimal Price { get; set; }

        public string Status { get; set; }

        public string ImageUrl { get; set; }
    }
}