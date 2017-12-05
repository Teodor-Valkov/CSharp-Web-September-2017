namespace CameraBazaar.Services.Contracts
{
    using Data.Models.Enums;
    using Models.Cameras;
    using System.Collections.Generic;

    public interface ICameraService
    {
        IEnumerable<CameraListServiceModel> GetAllListing(int page, int pageSize);

        CameraDetailsServiceModel GetCameraDetailsById(int id);

        CameraFormServiceModel GetCameraFormById(int id);

        void Create(
            Make make,
            string model,
            decimal price,
            int quantity,
            int minShutterSpeed,
            int maxShutterSpeed,
            MinISO minISO,
            int maxISO,
            bool isFullFrame,
            string videoResolution,
            IEnumerable<LightMetering> lightMeterings,
            string description,
            string imageUrl,
            string userId);

        void Edit(
            int id,
            Make make,
            string model,
            decimal price,
            int quantity,
            int minShutterSpeed,
            int maxShutterSpeed,
            MinISO minISO,
            int maxISO,
            bool isFullFrame,
            string videoResolution,
            IEnumerable<LightMetering> lightMeterings,
            string description,
            string imageUrl);

        void Delete(int id);

        bool IsUserOwner(string username, int id);

        int TotalCamerasCount();
    }
}