using System;
using CommonTypes;
using UserManagement.Models;

namespace Core.Models
{
    public class UserDetails
    {
        public UserAccountType AccountType { get; set; }
        public string AuthCode { get; set; }
        public string FullName { get; set; }
        public bool IsActive { get; set; }
        public string UserName { get; set; }
    }
}