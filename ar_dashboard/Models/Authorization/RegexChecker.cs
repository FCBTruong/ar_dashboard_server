using System.Text.RegularExpressions;

namespace ar_dashboard.Models
{
    public class RegexChecker
    {
        public static bool checkAuthString(string str)
        {
            Match myMatch = Regex.Match(str, @"(?!^[0-9]*$)(?!^[a-zA-Z]*$)^([a-zA-Z0-9]{6,15})$");
            if (myMatch.Success) return true;
            return false;
        }
    }
}
