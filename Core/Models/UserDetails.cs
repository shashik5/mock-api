using System;
using Authentication.Models;

namespace Core.Models
{
    public class UserDetails
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string DOB { get; set; }
        public AccountType AccountType { get; set; }
    }
}