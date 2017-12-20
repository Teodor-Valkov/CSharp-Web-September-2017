namespace FitStore.Tests.Web.Controllers
{
    using AutoMapper;
    using FitStore.Web.Infrastructure.Mapping;

    public class BaseControllerTest
    {
        static BaseControllerTest()
        {
            Mapper.Initialize(config => config.AddProfile<AutoMapperProfile>());
        }
    }
}