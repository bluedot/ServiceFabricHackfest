using StatelessBackendService.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApi.Controllers
{
    [ServiceRequestActionFilter]
    public class MessagesController : ApiController
    {

        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(System.Net.HttpStatusCode.OK, new List<Message> { new Message { Type = "xxx" } });
        }

        public HttpResponseMessage Post([FromBody] string message)
        {
            if (message != null)
            {
                //if (message.Type == "login")
                //{
                //    CallLoginService(message);
                //}
                //else if (message.Type == "putdata")
                //{
                //    CallPutDataService(message);
                //}
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest);
        }

        private void CallLoginService(Message message)
        {
            string serviceUri = WebApi.ServiceContext.CodePackageActivationContext.ApplicationName + "/LoginService";
            //LoginService proxy = ServiceProxy.Create<LonginService>(new Uri(serviceUri));
            //long result = await proxy.Login(message);
            //return this.Json(new { Count = result });
        }

        private void CallPutDataService(Message message)
        {
            throw new NotImplementedException();
        }
    }
}
