using System.Diagnostics;
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
            SimulateSlowEncode();

            // if no encoding, or current string is blank, just return input string
            if (provider == null || string.IsNullOrWhiteSpace(current))
            {
                return current;
            }

            // todo: find changed substring and only update those

            return provider.Encode(current);
        }

        [Conditional("DEBUG")]
        private static void SimulateSlowEncode()
        {
            Thread.Sleep(200);
        }
    }
}
