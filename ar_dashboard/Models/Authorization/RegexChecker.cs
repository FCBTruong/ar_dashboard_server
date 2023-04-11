using System.Text.RegularExpressions;

namespace ar_dashboard.Models
{
    public class RegexChecker
    {
        public static bool checkAuthString(string str)
        {
            Match myMatch = Regex.Match(str, @"([a-zA-Z0-9@.]{6,35})$");
            if (myMatch.Success) return true;
            return false;
        }
    }
}
