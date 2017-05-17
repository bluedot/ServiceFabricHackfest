using StatelessBackendService.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace LoginApi.Controllers
{


    [ServiceRequestActionFilter]
    public class LoginController : ApiController
    {
        public IHttpActionResult Post(LoginRequest loginRequest)
        {
            if(loginRequest == null)
            {
                return BadRequest();
            }

            if(loginRequest.Username == "user1")
            {
                return Ok();
            }

            return Unauthorized();
        }
    }
}
