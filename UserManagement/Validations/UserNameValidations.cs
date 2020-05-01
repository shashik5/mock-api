using System;
using System.Text.RegularExpressions;

namespace UserManagement.Validations
{
    static class UserNameValidation
    {
        private static string UserName = string.Empty;
        public static bool IsValid(string userName)
        {
            UserName = userName;
            return (IsInRange() && ContainsValidCharacters());
        }

        private static bool IsInRange()
        {
            int numberOfChars = UserName.Length;
            return (numberOfChars > 6 && numberOfChars < 16);
        }

        private static bool ContainsValidCharacters()
        {
            var regx = new Regex("^[\\w+@_!]+$");
            return regx.IsMatch(UserName);
        }
    }
}