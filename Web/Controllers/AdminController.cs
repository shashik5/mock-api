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
using Core.Models;
using CommonTypes;

namespace Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        [HttpPost]
        [ActionName("AuthenticateUser")]
        public ActionResult<AuthenticationResponseBody> AuthenticateUser()
        {
            StreamReader reader = new StreamReader(HttpContext.Request.Body);
            string requestFromPost = reader.ReadToEnd();
            AuthenticationRequestBody requestBody = JsonConvert.DeserializeObject<AuthenticationRequestBody>(requestFromPost.Replace("'", "\'"));
            string loginAuthCode = HttpContext.Request.Headers["authorization"];
            IApp core = GlobalApplicationData.GetGlobalData<IApp>(GlobalDataKey.Core);
            var userDetails = core.GetUserManager().AuthenticateUser(requestBody.UserName, loginAuthCode);
            return new AuthenticationResponseBody
            {
                AccountType = userDetails.AccountType,
                AuthCode = userDetails.AuthCode,
                FullName = userDetails.FullName,
                IsActive = userDetails.IsActive,
                UserName = userDetails.UserName
            };
        }

        [HttpPost]
        [ActionName("RegisterUser")]
        public ActionResult<RegisterUserResponse> RegisterUser()
        {
            StreamReader reader = new StreamReader(HttpContext.Request.Body);
            string requestFromPost = reader.ReadToEnd();
            RegisterUserRequestBody requestBody = JsonConvert.DeserializeObject<RegisterUserRequestBody>(requestFromPost.Replace("'", "\'"));
            IApp core = GlobalApplicationData.GetGlobalData<IApp>(GlobalDataKey.Core);
            var userActivationCode = core.GetUserManager().RegisterUser(requestBody);
            return new RegisterUserResponse
            {
                ActivationCode = userActivationCode
            };
        }

        [HttpPost]
        [ActionName("GetUserDetails")]
        public ActionResult<User> GetUserDetails()
        {
            string authCode = HttpContext.Request.Headers["authorization"];
            IApp core = GlobalApplicationData.GetGlobalData<IApp>(GlobalDataKey.Core);
            var userDetails = core.GetUserManager().GetUserDetailsByAuthCode(authCode);
            return new User
            {
                DOB = userDetails.DOB,
                Email = userDetails.Email,
                FullName = userDetails.FullName,
                IsActive = userDetails.IsActive,
                UserName = userDetails.UserName,
                AccountType = userDetails.AccountType
            };
        }
    }
}
