namespace FitStore.Tests.Mocks
{
    using Microsoft.AspNetCore.Identity;
    using Moq;

    public class RoleManagerMock
    {
        public static Mock<RoleManager<IdentityRole>> New()
        {
            return new Mock<RoleManager<IdentityRole>>(Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);
        }
    }
}