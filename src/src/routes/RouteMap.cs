using  HTTPServer.Controllers;

namespace  HTTPServer.Routing {
    public class RouteMap{
        public RouteMap(){
            // register the routes here you lizard
            Router.Path("/", BaseController.Index);
            Router.Path("/login", BaseController.Login);
        }

    }

}