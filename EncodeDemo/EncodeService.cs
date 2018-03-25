using System.Threading;

namespace EncodeDemo
{
    public class EncodeService
    {
        public static bool IsAlphanumeric(char ch)
        {
            if ((ch >= 'A' && ch <= 'Z') ||
                (ch >= 'a' && ch <= 'z') ||
                (ch >= '0' && ch <= '9'))
            {
                return true;
            }

            return false;
        }

        public static string Encode(string current, string last, string lastEncodedValue, IEncodeProvider provider)
        {
            Thread.Sleep(200);

            // if no encoding, or current string is blank, just return input string
            if (provider == null || string.IsNullOrWhiteSpace(current))
            {
                return last;
            }

            return last;
        }
    }
}
