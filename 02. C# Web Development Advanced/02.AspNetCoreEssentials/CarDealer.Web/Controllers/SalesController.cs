namespace CarDealer.Web.Controllers
{
    using Data.Models.Enums;
    using Infrastructure.Extensions;
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models.Sales;
    using Services.Contracts;
    using Services.Models.Cars;
    using Services.Models.Customers;
    using Services.Models.Sales;
    using System;
    using System.Linq;

    [Route("sales")]
    public class SalesController : Controller
    {
        private const int PageSize = 10;

        private readonly ISaleService saleService;
        private readonly ICustomerService customerService;
        private readonly ICarService carService;

        public SalesController(ISaleService saleService, ICustomerService customerService, ICarService carService)
        {
            this.saleService = saleService;
            this.customerService = customerService;
            this.carService = carService;
        }

        [Route("all")]
        public IActionResult All(int page = 1)
        {
            if (page < 1)
            {
                return RedirectToAction(nameof(All));
            }

            SalesPageListViewModel model = new SalesPageListViewModel
            {
                Sales = this.saleService.GetAllListing(page, PageSize),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(this.saleService.TotalSalesCount() / (double)PageSize)
            };

            if (page > model.TotalPages && model.TotalPages != 0)
            {
                return RedirectToAction(nameof(All), new { page = model.TotalPages });
            }

            return View(model);
        }

        [Route("discounted")]
        public IActionResult AllDiscountedSales(double? discount, int page = 1)
        {
            if (page < 1)
            {
                return RedirectToAction(nameof(AllDiscountedSales));
            }

            SalesPageListViewModel model = new SalesPageListViewModel
            {
                Sales = this.saleService.GetAllDiscountedListing(discount, page, PageSize),
                Discount = discount,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(this.saleService.TotalSalesWithDiscountCount(discount) / (double)PageSize)
            };

            if (page > model.TotalPages && model.TotalPages != 0)
            {
                return RedirectToAction(nameof(AllDiscountedSales), new { page = model.TotalPages });
            }

            return View(model);
        }

        [Route("{id}")]
        public IActionResult SaleById(int id)
        {
            SaleDetailsServiceModel model = this.saleService.GetSaleById(id);

            return this.ViewOrNotFound(model);
        }

        [Authorize]
        [Route("create")]
        public IActionResult Create()
        {
            SaleFormModel model = new SaleFormModel
            {
                AllCustomers = this.customerService
                    .GetAllBasicCustomers()
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                    .ToList(),
                AllCars = this.carService
                    .GetAllBasicCars()
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = $"{c.Make} {c.Model}"
                    })
                    .ToList(),
                AllDiscounts = Enumerable.Range(0, 100)
                    .Select(n => new SelectListItem
                    {
                        Value = n.ToString(),
                        Text = n.ToString()
                    })
                    .ToList()
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [Route("review")]
        public IActionResult Review(SaleFormModel saleModel)
        {
            CustomerBasicServiceModel customerModel = this.customerService.GetBasicCustomerById(saleModel.CustomerId);
            CarBasicServiceModel carModel = this.carService.GetBasicCarById(saleModel.CarId);

            SaleReviewFormModel model = new SaleReviewFormModel
            {
                CustomerId = customerModel.Id,
                CustomerName = customerModel.Name,
                IsYoungDriver = customerModel.IsYoungDriver,
                CarId = carModel.Id,
                CarMake = $"{carModel.Make} {carModel.Model}",
                Discount = saleModel.Discount,
                CarPrice = this.saleService.GetCarPrice(carModel.Id),
                FinalCarPrice = this.saleService.GetCarPriceWithDiscount(carModel.Id, saleModel.Discount, customerModel.IsYoungDriver)
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [LogFilter(Operation.Add, ModifiedTable.Sale)]
        [Route("finalize")]
        public IActionResult Finalize(SaleReviewFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Review), model);
            }

            this.saleService.Create(model.CustomerId, model.CarId, model.Discount);

            return RedirectToAction(nameof(All));
        }
    }
}