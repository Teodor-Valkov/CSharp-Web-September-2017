namespace CarDealer.Web.Infrastructure.Filters
{
    using Data.Models.Enums;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.DependencyInjection;
    using Services.Contracts;
    using System;

    public class LogFilterAttribute : ActionFilterAttribute
    {
        private readonly Operation operation;
        private readonly ModifiedTable modifiedTable;

        public LogFilterAttribute(Operation operation, ModifiedTable modifiedTable)
        {
            this.operation = operation;
            this.modifiedTable = modifiedTable;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            ILogService logService = context.HttpContext.RequestServices.GetService<ILogService>();

            logService.Create(context.HttpContext.User.Identity.Name, this.operation, this.modifiedTable, DateTime.Now);
        }
    }
}