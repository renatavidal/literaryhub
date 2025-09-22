// Utils/CardUtils.cs
using System.Linq;
using System.Text.RegularExpressions;

namespace Servicios   // o Common/Util, pero usá siempre el mismo namespace en los 3 lugares
{
    public static class CardUtils
    {
        public static string OnlyDigits(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return new string(s.Where(char.IsDigit).ToArray());
        }

        public static string DetectBrand(string pan)
        {
            if (Regex.IsMatch(pan, @"^4\d{12}(\d{3})?(\d{3})?$")) return "VISA";
            if (Regex.IsMatch(pan, @"^(5[1-5]\d{4}|222[1-9]\d{2}|22[3-9]\d{3}|2[3-6]\d{4}|27[01]\d{3}|2720\d{2})\d{10}$")) return "MASTERCARD";
            if (Regex.IsMatch(pan, @"^3[47]\d{13}$")) return "AMEX";
            return "CARD";
        }
    }
}
