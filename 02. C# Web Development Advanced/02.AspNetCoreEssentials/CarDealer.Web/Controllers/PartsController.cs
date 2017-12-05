namespace CarDealer.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models.Parts;
    using Services.Contracts;
    using Services.Models.Parts;
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public class PartsController : Controller
    {
        private const int PageSize = 25;

        private readonly IPartService partService;
        private readonly ISupplierService supplierService;

        public PartsController(IPartService partService, ISupplierService supplierService)
        {
            this.partService = partService;
            this.supplierService = supplierService;
        }

        public IActionResult All(int page = 1)
        {
            if (page < 1)
            {
                return RedirectToAction(nameof(All));
            }

            PartsPageListViewModel model = new PartsPageListViewModel
            {
                Parts = this.partService.GetAllListing(page, PageSize),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(this.partService.TotalPartsCount() / (double)PageSize)
            };

            if (page > model.TotalPages && model.TotalPages != 0)
            {
                return RedirectToAction(nameof(All), new { page = model.TotalPages });
            }

            return View(model);
        }

        public IActionResult Create()
        {
            PartFormModel model = new PartFormModel
            {
                Suppliers = this.GetAllSupplierListItems()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(PartFormModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Suppliers = this.GetAllSupplierListItems();

                return View(model);
            }

            this.partService.Create(model.Name, model.Quantity, model.Price, model.SupplierId);

            return RedirectToAction(nameof(All));
        }

        public IActionResult Edit(int id)
        {
            PartDetailsServiceModel part = this.partService.GetPartById(id);

            if (part == null)
            {
                return NotFound();
            }

            PartFormModel model = new PartFormModel
            {
                Name = part.Name,
                Price = part.Price,
                Quantity = part.Quantity,
                IsEdit = true
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(int id, PartFormModel model)
        {
            if (!ModelState.IsValid)
            {
                model.IsEdit = true;

                return View(model);
            }

            this.partService.Edit(id, model.Price, model.Quantity);

            return RedirectToAction(nameof(All));
        }

        public IActionResult Delete(int id)
        {
            return View(id);
        }

        public IActionResult Confirm(int id)
        {
            this.partService.Delete(id);

            return RedirectToAction(nameof(All));
        }

        private IEnumerable<SelectListItem> GetAllSupplierListItems()
        {
            return this.supplierService
                .GetAllPartSuppliers()
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                });
        }
    }
}