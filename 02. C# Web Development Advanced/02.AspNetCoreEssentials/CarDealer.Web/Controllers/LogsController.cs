namespace CarDealer.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models.Logs;
    using Services.Contracts;
    using System;

    public class LogsController : Controller
    {
        private const int PageSize = 5;

        private ILogService logService;

        public LogsController(ILogService logService)
        {
            this.logService = logService;
        }

        [Authorize]
        public IActionResult All(string username, int page = 1)
        {
            if (page < 1)
            {
                return RedirectToAction(nameof(All));
            }

            LogsPageListViewModel model = new LogsPageListViewModel
            {
                Logs = this.logService.GetAllListing(username, page, PageSize),
                Username = username,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(this.logService.TotalLogsCount(username) / (double)PageSize)
            };

            if (page > model.TotalPages && model.TotalPages != 0)
            {
                return RedirectToAction(nameof(All), new { page = model.TotalPages });
            }

            return View(model);
        }

        [Authorize]
        public IActionResult Clear()
        {
            this.logService.Clear();

            return RedirectToAction(nameof(All));
        }
    }
}