using System;
using Core.Models;

namespace Web.Models
{
    public class AuthenticationRequestBody
    {
        public string UserName { get; set; } = string.Empty;
    }

    public class AuthenticationResponseBody : UserDetails
    {
    }
}