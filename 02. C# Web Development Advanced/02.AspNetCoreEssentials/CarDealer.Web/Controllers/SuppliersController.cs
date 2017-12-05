namespace CarDealer.Web.Controllers
{
    using CarDealer.Web.Infrastructure.Filters;
    using Data.Models.Enums;
    using Microsoft.AspNetCore.Mvc;
    using Models.Suppliers;
    using Services.Contracts;
    using Services.Models.Suppliers;
    using System;
    using System.Collections.Generic;

    public class SuppliersController : Controller
    {
        private const int PageSize = 10;

        private readonly ISupplierService supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            this.supplierService = supplierService;
        }

        public IActionResult All(int page = 1)
        {
            if (page < 1)
            {
                return RedirectToAction(nameof(All));
            }

            SuppliersPageListViewModel model = new SuppliersPageListViewModel
            {
                Suppliers = this.supplierService.GetAllListing(page, PageSize),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(this.supplierService.TotalSuppliersCount() / (double)PageSize)
            };

            if (page > model.TotalPages && model.TotalPages != 0)
            {
                return RedirectToAction(nameof(All), new { page = model.TotalPages });
            }

            return View(model);
        }

        public IActionResult Local()
        {
            return View("SupplierByType", this.GetSuppliersByTypeImporter(false));
        }

        public IActionResult Importers()
        {
            return View("SupplierByType", this.GetSuppliersByTypeImporter(true));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [LogFilter(Operation.Add, ModifiedTable.Supplier)]
        public IActionResult Create(SupplierFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            this.supplierService.Create(model.Name, model.IsImporter);

            return RedirectToAction((nameof(All)));
        }

        public IActionResult Edit(int id)
        {
            SupplierDetailsServiceModel supplier = this.supplierService.GetSupplierById(id);

            if (supplier == null)
            {
                return NotFound();
            }

            SupplierFormModel model = new SupplierFormModel
            {
                Name = supplier.Name,
                IsImporter = supplier.IsImporter,
                IsEdit = true
            };

            return View(model);
        }

        [HttpPost]
        [LogFilter(Operation.Edit, ModifiedTable.Supplier)]
        public IActionResult Edit(int id, SupplierFormModel model)
        {
            if (!ModelState.IsValid)
            {
                model.IsEdit = true;

                return View(model);
            }

            this.supplierService.Edit(id, model.Name, model.IsImporter);

            return RedirectToAction(nameof(All));
        }

        public IActionResult Delete(int id)
        {
            return View(id);
        }

        [LogFilter(Operation.Delete, ModifiedTable.Supplier)]
        public IActionResult Confirm(int id)
        {
            this.supplierService.Delete(id);

            return RedirectToAction(nameof(All));
        }

        private SuppliersViewModel GetSuppliersByTypeImporter(bool isImporter)
        {
            string type = isImporter ? "Importer" : "Local";

            IEnumerable<SuppliersListServiceModel> suppliers = this.supplierService.GetAllByType(isImporter);

            SuppliersViewModel model = new SuppliersViewModel
            {
                Type = type,
                Suppliers = suppliers
            };

            return model;
        }
    }
}