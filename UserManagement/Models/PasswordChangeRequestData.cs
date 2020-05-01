using System;

namespace UserManagement.Models
{
    public class PasswordChangeRequestData
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string UserName { get; set; }
    }
}
