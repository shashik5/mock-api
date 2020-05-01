using System;
using Core.Models;

namespace Web.Models
{
    public class RegisterUserRequestBody : User
    {
    }

    public class RegisterUserResponse
    {
        public string ActivationCode { get; set; }
    }
}