using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApi.Controllers
{
    [ServiceRequestActionFilter]
    public class TestController: ApiController
    {
        public HttpResponseMessage Get()
        {
            var indexPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "wwwroot/index.html");
            using ( var stream = File.OpenRead(indexPath))
            {
                var response = new HttpResponseMessage();
                response.Content = new StringContent(File.ReadAllText(indexPath));
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                return response;
            }

        }
    }
}
