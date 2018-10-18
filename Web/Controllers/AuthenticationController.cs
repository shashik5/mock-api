using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Models;
using Web.Data;
using Core;

namespace Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        [HttpPost]
        [ActionName("AuthorizeUser")]
        public ActionResult<AuthenticationResponseBody> AuthorizeUser()
        {
            StreamReader reader = new StreamReader(HttpContext.Request.Body);
            string requestFromPost = reader.ReadToEnd();
            AuthenticationRequestBody requestBody = JsonConvert.DeserializeObject<AuthenticationRequestBody>(requestFromPost.Replace("'", "\'"));
            string authCode = HttpContext.Request.Headers["authorization"];
            App core = GlobalApplicationData.GetGlobalData<App>(GlobalDataKey.Core);

            if (!core.ValidateUser(requestBody.UserName, authCode))
            {
                throw new Exception("Invalid Credentials");
            };

            return (AuthenticationResponseBody)core.GetUserDetails(authCode);
        }
    }
}
