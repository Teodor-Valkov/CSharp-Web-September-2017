namespace FitStore.Web.Areas.Manager.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using static Common.CommonConstants;
    using static WebConstants;

    [Area(ManagerArea)]
    [Authorize(Roles = AdministratorRole)]
    public class BaseManagerController : Controller
    {
    }
}