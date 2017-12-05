namespace CameraBazaar.Services.Implementations
{
    using Contracts;
    using Data;
    using Data.Common;
    using Data.Models;
    using Data.Models.Enums;
    using Models.Cameras;
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using Microsoft.AspNetCore.Identity;

    public class CameraService : ICameraService
    {
        private readonly CameraBazaarDbContext database;
        private UserManager<User> userManager;

        public CameraService(CameraBazaarDbContext database, UserManager<User> userManager)
        {
            this.database = database;
            this.userManager = userManager;
        }

        public IEnumerable<CameraListServiceModel> GetAllListing(int page, int pageSize)
        {
            return this.database
                .Cameras
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CameraListServiceModel
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    Price = c.Price,
                    Status = c.Quantity != 0 ? MessageConstants.InStockMessage : MessageConstants.OutOfStockMessage,
                    ImageUrl = c.ImageUrl
                })
                .ToList();
        }

        public CameraDetailsServiceModel GetCameraDetailsById(int id)
        {
            return this.database
                .Cameras
                .Where(c => c.Id == id)
                .Select(c => new CameraDetailsServiceModel
                {
                    Make = c.Make,
                    Model = c.Model,
                    Price = c.Price,
                    Quantity = c.Quantity,
                    MinShutterSpeed = c.MinShutterSpeed,
                    MaxShutterSpeed = c.MaxShutterSpeed,
                    MinISO = c.MinISO,
                    MaxISO = c.MaxISO,
                    FullFrame = c.IsFullFrame ? "Yes" : "No",
                    VideoResolution = c.VideoResolution,
                    LightMeterings = Enum.GetValues(typeof(LightMetering)).Cast<LightMetering>().Where(flag => c.LightMetering.HasFlag(flag)).ToList(),
                    ImageUrl = c.ImageUrl,
                    Description = c.Description,
                    Status = c.Quantity != 0 ? MessageConstants.InStockMessage : MessageConstants.OutOfStockMessage,
                    SellerUsername = c.User.UserName
                })
                .FirstOrDefault();
        }

        public CameraFormServiceModel GetCameraFormById(int id)
        {
            return this.database
               .Cameras
               .Where(c => c.Id == id)
               .Select(c => new CameraFormServiceModel
               {
                   Make = c.Make,
                   Model = c.Model,
                   Price = c.Price,
                   Quantity = c.Quantity,
                   MinShutterSpeed = c.MinShutterSpeed,
                   MaxShutterSpeed = c.MaxShutterSpeed,
                   MinISO = c.MinISO,
                   MaxISO = c.MaxISO,
                   IsFullFrame = c.IsFullFrame,
                   VideoResolution = c.VideoResolution,
                   LightMeterings = Enum.GetValues(typeof(LightMetering)).Cast<LightMetering>().Where(flag => c.LightMetering.HasFlag(flag)).ToList(),
                   ImageUrl = c.ImageUrl,
                   Description = c.Description
               })
               .FirstOrDefault();
        }

        public void Create(
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
            string userId)
        {
            Camera camera = new Camera
            {
                Make = make,
                Model = model,
                Price = price,
                Quantity = quantity,
                MinShutterSpeed = minShutterSpeed,
                MaxShutterSpeed = maxShutterSpeed,
                MinISO = minISO,
                MaxISO = maxISO,
                IsFullFrame = isFullFrame,
                VideoResolution = videoResolution,
                LightMetering = (LightMetering)lightMeterings.Cast<int>().Sum(),
                ImageUrl = imageUrl,
                Description = description,
                UserId = userId
            };

            this.database.Cameras.Add(camera);
            this.database.SaveChanges();
        }

        public void Edit(
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
            string imageUrl)
        {
            Camera camera = this.database.Cameras.Find(id);

            if (camera == null)
            {
                return;
            }

            camera.Make = make;
            camera.Model = model;
            camera.Price = price;
            camera.Quantity = quantity;
            camera.MinShutterSpeed = minShutterSpeed;
            camera.MaxShutterSpeed = maxShutterSpeed;
            camera.MinISO = minISO;
            camera.MaxISO = maxISO;
            camera.IsFullFrame = isFullFrame;
            camera.VideoResolution = videoResolution;
            camera.LightMetering = (LightMetering)lightMeterings.Cast<int>().Sum();
            camera.Description = description;
            camera.ImageUrl = imageUrl;

            this.database.SaveChanges();
        }

        public void Delete(int id)
        {
            Camera camera = this.database.Cameras.Find(id);

            if (camera == null)
            {
                return;
            }

            this.database.Cameras.Remove(camera);
            this.database.SaveChanges();
        }

        public bool IsUserOwner(string username, int id)
        {
            return this.database.Cameras.Any(c => c.Id == id && c.User.UserName.ToLower() == username.ToLower());
        }

        public int TotalCamerasCount()
        {
            return this.database.Cameras.Count();
        }
    }
}