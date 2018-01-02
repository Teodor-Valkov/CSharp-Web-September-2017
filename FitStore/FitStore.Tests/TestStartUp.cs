namespace FitStore.Tests
{
    using AutoMapper;
    using FitStore.Web.Infrastructure.Mapping;

    // A way of initializing the mapper configuration only once - using lock (other way is with static constructor)
    //
    public class TestStartup
    {
        private static object sync = new object();
        private static bool mapperInitialized = false;

        public static void Initialize()
        {
            lock (sync)
            {
                if (!mapperInitialized)
                {
                    Mapper.Initialize(config => config.AddProfile<AutoMapperProfile>());

                    mapperInitialized = true;
                }
            }
        }
    }
}