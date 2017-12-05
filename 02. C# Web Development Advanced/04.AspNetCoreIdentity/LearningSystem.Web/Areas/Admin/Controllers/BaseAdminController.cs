namespace LearningSystem.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using static Common.CommonConstants;
    using static WebConstants;

    [Area(AdminArea)]
    [Authorize(Roles = AdministratorRole)]
    public abstract class BaseAdminController : Controller
    {
    }
}