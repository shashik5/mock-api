using System;
using System.Text.RegularExpressions;

namespace Utilities
{
    public class EmailDetails
    {
        public string Body { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string To { get; set; }
        public string Cc { get; set; } = string.Empty;
    }

    public static class HelperMethods
    {
        public static string GenerateUniqueID()
        {
            var uniqueID = Guid.NewGuid().ToString();
            return Regex.Replace(uniqueID, "-", "");
        }

        public static bool SendEmail(EmailDetails emailDetails)
        {
            return true;
        }
    }
}
