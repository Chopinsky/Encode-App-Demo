using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace EncodeDemo
{
    public class EncodeService
    {
        /// <summary>
        /// Check if a character is an English alphanumeric
        /// </summary>
        /// <param name="ch">Input character to check with</param>
        /// <returns>True if the input is an alphanumeric</returns>
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
        
        /// <summary>
        /// Generate the encode map table with formats
        /// </summary>
        /// <param name="provider">Encode provider that implements the IEncodeProvider</param>
        /// <returns>The string content of the encode map table</returns>
        public static string GenerateShowMapTable(IEncodeProvider provider)
        {
            if (provider == null)
            {
                return ">> No encoding provider is set <<";
            }

            var encodeMap = provider.GetEncodeMap();
            if (encodeMap == null || encodeMap.Count == 0)
            {
                return ">> No encoding map is provided <<";
            }

            var tableContent = new StringBuilder();
            var count = 0;

            foreach (KeyValuePair<char, char> pair in encodeMap)
            {
                if (tableContent.Length > 0)
                {
                    // if we already have contents, append separators.
                    if (count % 10 == 0)
                    {
                        tableContent.Append(",\r\n");
                    }
                    else
                    {
                        tableContent.Append(",\t");
                    }
                }

                tableContent.AppendFormat("{0}={1}", pair.Key, pair.Value);
                count++;
            }

            tableContent.Append('.');       // ending the table with dot
            return tableContent.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static string Encode(string current, IEncodeProvider provider)
        {
            SimulateSlowEncode();

            // if no encoding, or current string is blank, just return input string
            if (provider == null || string.IsNullOrWhiteSpace(current))
            {
                return current;
            }

            return provider.Encode(current);
        }

        [Conditional("DEBUG")]
        private static void SimulateSlowEncode()
        {
            if (Properties.Settings.Default.ApplyAsyncDelay)
            {
                Thread.Sleep(200);
            }
        }
    }
}
