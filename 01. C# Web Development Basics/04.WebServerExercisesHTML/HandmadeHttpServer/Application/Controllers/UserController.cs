namespace HandmadeHttpServer.Application.Controllers
{
    using HandmadeHttpServer.Application.Views.User;
    using HandmadeHttpServer.Server.Enums;
    using HandmadeHttpServer.Server.Http.Contracts;
    using HandmadeHttpServer.Server.Http.Response;
    using HandmadeHttpServer.Server.Models;

    public class UserController
    {
        // Get /register
        public IHttpResponse RegisterGet()
        {
            return new ViewResponse(HttpStatusCode.Ok, new RegisterView());
        }

        //Post /register
        public IHttpResponse RegisterPost(string name)
        {
            return new RedirectResponse($"/user/{name}");
        }

        // Get /user/name
        public IHttpResponse Details(string name)
        {
            UserModel userModel = new UserModel();
            userModel.AddObject("name", userModel);

            return new ViewResponse(HttpStatusCode.Ok, new DetailsView(userModel));
        }
    }
}