namespace CameraBazaar.Web.Controllers
{
    using Data.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models.Cameras;
    using Services.Contracts;
    using Services.Models.Cameras;
    using System;

    public class CamerasController : Controller
    {
        private const int PageSize = 2;

        private readonly UserManager<User> userManager;
        private readonly ICameraService cameraService;
        private readonly IUserService userService;

        public CamerasController(UserManager<User> userManager, ICameraService cameraService, IUserService userService)
        {
            this.userManager = userManager;
            this.cameraService = cameraService;
            this.userService = userService;
        }

        public IActionResult All(int page = 1)
        {
            if (page < 1)
            {
                return RedirectToAction(nameof(All));
            }

            CameraPageListViewModel model = new CameraPageListViewModel
            {
                Cameras = this.cameraService.GetAllListing(page, PageSize),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(this.cameraService.TotalCamerasCount() / (double)PageSize)
            };

            if (page > model.TotalPages && model.TotalPages != 0)
            {
                return RedirectToAction(nameof(All), new { page = model.TotalPages });
            }

            return View(model);
        }

        public IActionResult Details(int id)
        {
            CameraDetailsServiceModel model = this.cameraService.GetCameraDetailsById(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [Authorize]
        public IActionResult Create()
        {
            bool isUserAllowedToCreateCameras = this.userService.IsUserAllowedToCreateCameras(this.userManager.GetUserId(User));

            if (!isUserAllowedToCreateCameras)
            {
                return RedirectToAction(nameof(AccountController.AccessDenied), "Account");
            }

            CameraFormViewModel model = new CameraFormViewModel();

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CameraFormViewModel cameraModel)
        {
            bool isUserAllowedToCreateCameras = this.userService.IsUserAllowedToCreateCameras(this.userManager.GetUserId(User));

            if (!isUserAllowedToCreateCameras)
            {
                return RedirectToAction(nameof(AccountController.AccessDenied), "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(cameraModel);
            }

            this.cameraService.Create(
                cameraModel.Make,
                cameraModel.Model,
                cameraModel.Price,
                cameraModel.Quantity,
                cameraModel.MinShutterSpeed,
                cameraModel.MaxShutterSpeed,
                cameraModel.MinISO,
                cameraModel.MaxISO,
                cameraModel.IsFullFrame,
                cameraModel.VideoResolution,
                cameraModel.LightMeterings == null ? null : cameraModel.LightMeterings,
                cameraModel.Description,
                cameraModel.ImageUrl,
                this.userManager.GetUserId(User)
                );

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [Authorize]
        public IActionResult Edit(int id)
        {
            string username = User.Identity.Name;

            if (!this.cameraService.IsUserOwner(username, id))
            {
                return RedirectToAction(nameof(All));
            }

            CameraFormServiceModel camera = this.cameraService.GetCameraFormById(id);

            if (camera == null)
            {
                return NotFound();
            }

            CameraFormViewModel model = new CameraFormViewModel
            {
                Make = camera.Make,
                Model = camera.Model,
                Price = camera.Price,
                Quantity = camera.Quantity,
                MinShutterSpeed = camera.MinShutterSpeed,
                MaxShutterSpeed = camera.MaxShutterSpeed,
                MinISO = camera.MinISO,
                MaxISO = camera.MaxISO,
                IsFullFrame = camera.IsFullFrame,
                VideoResolution = camera.VideoResolution,
                LightMeterings = camera.LightMeterings,
                Description = camera.Description,
                ImageUrl = camera.ImageUrl,
                IsEdit = true
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CameraFormViewModel cameraModel)
        {
            string username = User.Identity.Name;

            if (!this.cameraService.IsUserOwner(username, id))
            {
                return RedirectToAction(nameof(All));
            }

            if (!ModelState.IsValid)
            {
                return View(cameraModel);
            }

            this.cameraService.Edit(
                id,
                cameraModel.Make,
                cameraModel.Model,
                cameraModel.Price,
                cameraModel.Quantity,
                cameraModel.MinShutterSpeed,
                cameraModel.MaxShutterSpeed,
                cameraModel.MinISO,
                cameraModel.MaxISO,
                cameraModel.IsFullFrame,
                cameraModel.VideoResolution,
                cameraModel.LightMeterings,
                cameraModel.Description,
                cameraModel.ImageUrl);

            return this.RedirectToAction(nameof(Details), new { id = id });
        }

        [Authorize]
        public IActionResult Delete(int id)
        {
            string username = User.Identity.Name;

            if (!this.cameraService.IsUserOwner(username, id))
            {
                return RedirectToAction(nameof(All));
            }

            return View(id);
        }

        [Authorize]
        public IActionResult ConfirmDelete(int id)
        {
            string username = User.Identity.Name;

            if (!this.cameraService.IsUserOwner(username, id))
            {
                return RedirectToAction(nameof(All));
            }

            this.cameraService.Delete(id);

            return RedirectToAction(nameof(All));
        }
    }
}