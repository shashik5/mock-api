using System;
using System.Text.RegularExpressions;

namespace UserManagement.Validations
{
    static class EmailValidations
    {
        public static bool IsValid(string email)
        {
            var regx = new Regex("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$");
            return regx.IsMatch(email);
        }
    }
}