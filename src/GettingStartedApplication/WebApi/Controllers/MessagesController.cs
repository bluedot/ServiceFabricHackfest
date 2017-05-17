using StatelessBackendService.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net;
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

            //if (message != null)
            //{
            //    if (message.Type == "login")
            //    {
            //        return await CallLoginService(message);
            //    }
            //    //else if (message.Type == "putdata")
            //    //{
            //    //    CallPutDataService(message);
            //    //}
            //    return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest));

            //    return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            //}
            //return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest);
        }

        private async Task<HttpResponseMessage> CallLoginService(Message message)
        {
            using (var client = GetLoginClient())
            {
                var loginRequest = new LoginRequest
                {
                    Username = message.Payload["username"].ToString(),
                    Password = message.Payload["password"].ToString()
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
            throw new NotImplementedException();
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
