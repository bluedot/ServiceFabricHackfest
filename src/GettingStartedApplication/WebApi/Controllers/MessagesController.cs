using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using StatelessBackendService.Interfaces;
using StatelessBackendService.Interfaces.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApi.Controllers
{
    [ServiceRequestActionFilter]
    public class MessagesController : ApiController
    {
        public async Task<HttpResponseMessage> Get()
        {
            //return Request.CreateResponse(System.Net.HttpStatusCode.OK, new List<Message> { new Message { Type = "xxx" } });

            string serviceUri = WebApi.ServiceContext.CodePackageActivationContext.ApplicationName + "/StatefulBackendService";
            IQueueAdapter proxy = ServiceProxy.Create<IQueueAdapter>(new Uri(serviceUri));
            var result = await proxy.Pop();
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        public async Task<HttpResponseMessage> Post(Message message)
        {
            switch(message.Type)
            {
                case "login":
                    return await CallLoginService(message);
                case "putdata":
                    return await CallPutDataService(message);
                default:
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
            }            
        }

        private async Task<HttpResponseMessage> CallLoginService(Message message)
        {
            using (var client = GetLoginClient())
            {
                var loginRequest = new LoginRequest
                {
                    //Username = message.Payload["username"].ToString(),
                    //Password = message.Payload["password"].ToString()
                };

                var response = await client.PostAsJsonAsync("login", loginRequest);

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                return Request.CreateErrorResponse(response.StatusCode, new Exception(content));
            }
        }

        private async Task<HttpResponseMessage> CallPutDataService(Message message)
        {
            string serviceUri = WebApi.ServiceContext.CodePackageActivationContext.ApplicationName + "/StatefulBackendService";
            IQueueAdapter proxy = ServiceProxy.Create<IQueueAdapter>(new Uri(serviceUri));
            var result = await proxy.Push(message);
            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        private HttpClient GetLoginClient()
        {
            return new HttpClient()
            {
                BaseAddress = new Uri( "http://localhost:9054/api/")
            };
        }
    }
}
