namespace FitStore.Tests.Web
{
    public abstract class BaseControllerTest
    {
        // A way of initializing the mapper configuration only once - using lock (other way is with static constructor like below)
        //
        protected BaseControllerTest()
        {
            TestStartup.Initialize();
        }

        //static BaseControllerTest()
        //{
        //    Mapper.Initialize(config => config.AddProfile<AutoMapperProfile>());
        //}
    }
}