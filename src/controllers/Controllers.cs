using System.Net.Sockets;
using Serilog;

namespace   HTTPServer.Controllers {



    public static class BaseController {

        public static void Index(Connection request){
            
            request.HTMLDocumentResponse("index.html"); 
        }
        public static void Login(Connection  request ){
            request.HTMLDocumentResponse("login.html");
        }
    }

}   
