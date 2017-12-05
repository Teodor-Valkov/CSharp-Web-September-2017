namespace CarDealer.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Models.Customers;
    using Services.Contracts;
    using Services.Models.Customers;
    using Services.Models.Enums;
    using System;

    [Route("customers")]
    public class CustomersController : Controller
    {
        private const int PageSize = 10;

        private readonly ICustomerService customerService;

        public CustomersController(ICustomerService customerService)
        {
            this.customerService = customerService;
        }

        [Route("all/{order}")]
        public IActionResult All(string order, int page = 1)
        {
            OrderDirection orderDirection = order.ToLower() == "descending"
                ? OrderDirection.Descending
                : OrderDirection.Ascending;

            if (page < 1)
            {
                return RedirectToAction(nameof(All));
            }

            CustomersPageListViewModel model = new CustomersPageListViewModel
            {
                OrderDirection = orderDirection,
                Customers = this.customerService.GetOrderedCustomersListing(orderDirection, page, PageSize),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(this.customerService.TotalCustomersCount() / (double)PageSize)
            };

            if (page > model.TotalPages && model.TotalPages != 0)
            {
                return RedirectToAction(nameof(All), new { page = model.TotalPages });
            }

            return View(model);
        }

        [Route("{id}")]
        public IActionResult TotalSalesById(int id)
        {
            CustomerWithSalesServiceModel model = this.customerService.GetCustomerWithSalesById(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [Route(nameof(Create))]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Route(nameof(Create))]
        public IActionResult Create(CustomerFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            this.customerService.Create(model.Name, model.BirthDate, model.IsYoungDriver);

            return RedirectToAction(nameof(All), new { order = OrderDirection.Ascending });
        }

        [Route(nameof(Edit) + "/{id}")]
        public IActionResult Edit(int id)
        {
            CustomerServiceModel customer = this.customerService.GetCustomerById(id);

            if (customer == null)
            {
                return NotFound();
            }

            CustomerFormModel model = new CustomerFormModel
            {
                Name = customer.Name,
                BirthDate = customer.BirthDate,
                IsYoungDriver = customer.IsYoungDriver
            };

            return View(model);
        }

        [HttpPost]
        [Route(nameof(Edit) + "/{id}")]
        public IActionResult Edit(int id, CustomerFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!this.customerService.IsCustomerExisting(id))
            {
                return NotFound();
            }

            this.customerService.Edit(id, model.Name, model.BirthDate, model.IsYoungDriver);

            return RedirectToAction(nameof(All), new { order = OrderDirection.Ascending });
        }
    }
}