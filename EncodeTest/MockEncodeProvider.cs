using EncodeDemo;
using System;
using System.Collections.Generic;
using System.Text;

namespace EncodeTestSupport
{
    /// <summary>
    /// Mock encode provider: convert capital case English characters to lower case characters, and vice versa.
    /// </summary>
    public class MockEncodeProvider: IEncodeProvider
    {
        public MockEncodeProvider(bool generateTableNow = true)
        {
            if (generateTableNow)
            {
                generateEncodeTable();
            }            
        }

        private Dictionary<char, char> encodeTable = new Dictionary<char, char>();

        public Dictionary<char, char> GetEncodeMap()
        {
            return encodeTable;
        }

        public void RegenerateEncodeTable()
        {
            encodeTable.Clear();
            generateEncodeTable();
        }

        public string Encode(string str)
        {
            var builder = new StringBuilder(str.Length);

            foreach (char ch in str)
            {
                if (encodeTable.ContainsKey(ch))
                {
                    builder.Append(encodeTable[ch]);
                }
                else
                {
                    builder.Append(ch);
                }
            }

            return builder.ToString();
        }

        private void generateEncodeTable()
        {
            int diff = 'a' - 'A';
            char mapped;

            foreach (char ch in GetNextAlphanumeric())
            {
                if (char.IsUpper(ch))
                {
                    // mapping capital characters to lower characters
                    mapped = Convert.ToChar(ch + diff);
                    encodeTable.Add(ch, mapped);
                }
                else
                {
                    // mapping lower characters to capital characters
                    mapped = Convert.ToChar(ch - diff);
                    encodeTable.Add(ch, mapped);
                }
            }
        }

        private IEnumerable<char> GetNextAlphanumeric()
        {
            for (char ch = 'a'; ch <= 'z'; ch++)
            {
                yield return ch;
            }

            for (char ch = 'A'; ch <= 'Z'; ch++)
            {
                yield return ch;
            }
        }
    }
}
