namespace CarDealer.Web.Controllers
{
    using Data.Models.Enums;
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models.Cars;
    using Services.Contracts;
    using Services.Models.Cars;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    [Route("cars")]
    public class CarsController : Controller
    {
        private const int PageSize = 25;

        private readonly ICarService carService;
        private readonly IPartService partService;

        public object CarPageListViewModel { get; private set; }

        public CarsController(ICarService carService, IPartService partService)
        {
            this.carService = carService;
            this.partService = partService;
        }

        [Route("all")]
        public IActionResult All(int page = 1)
        {
            if (page < 1)
            {
                return RedirectToAction(nameof(All));
            }

            CarsPageListViewModel model = new CarsPageListViewModel
            {
                Cars = this.carService.GetAllListing(page, PageSize),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(this.carService.TotalCarsCount() / (double)PageSize)
            };

            if (page > model.TotalPages && model.TotalPages != 0)
            {
                return RedirectToAction(nameof(All), new { page = model.TotalPages });
            }

            return View(model);
        }

        [Route("{make}")]
        public IActionResult AllByMake(string make)
        {
            IEnumerable<CarServiceModel> cars = this.carService.GetAllByMake(make);

            CarsByMakeViewModel model = new CarsByMakeViewModel
            {
                Make = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(make.ToLower()),
                Cars = cars
            };

            return View(model);
        }

        [Authorize]
        [Route(nameof(Create))]
        public IActionResult Create()
        {
            CarFormModel model = new CarFormModel
            {
                AllParts = this.GetAllPartSelectItems()
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [LogFilter(Operation.Add, ModifiedTable.Car)]
        [Route(nameof(Create))]
        public IActionResult Create(CarFormModel formModel)
        {
            if (!ModelState.IsValid)
            {
                formModel.AllParts = this.GetAllPartSelectItems();

                return View(formModel);
            }

            this.carService.Create(formModel.Make, formModel.Model, formModel.TravelledDistance, formModel.PartsIds);

            return RedirectToAction(nameof(All));
        }

        [Route("details/{id}")]
        public IActionResult Details(int id)
        {
            CarWithPartsServiceModel model = this.carService.GetCarWithPartsById(id);

            return View(model);
        }

        private IEnumerable<SelectListItem> GetAllPartSelectItems()
        {
            return this.partService
                .GetAllParts()
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                });
        }
    }
}