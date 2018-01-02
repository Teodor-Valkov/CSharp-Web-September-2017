namespace FitStore.Tests.Services
{
    using Data;
    using Microsoft.EntityFrameworkCore;
    using System;

    public abstract class BaseServiceTest
    {
        // A way of initializing the mapper configuration only once - using lock (other way is with static constructor like below)
        //
        protected BaseServiceTest()
        {
            TestStartup.Initialize();
        }

        //static BaseServiceTest()
        //{
        //    Mapper.Initialize(config => config.AddProfile<AutoMapperProfile>());
        //}

        protected FitStoreDbContext Database
        {
            get
            {
                DbContextOptions<FitStoreDbContext> databaseContextOptions = new DbContextOptionsBuilder<FitStoreDbContext>()
                   .UseInMemoryDatabase(Guid.NewGuid().ToString())
                   .Options;

                return new FitStoreDbContext(databaseContextOptions);
            }
        }
    }
}