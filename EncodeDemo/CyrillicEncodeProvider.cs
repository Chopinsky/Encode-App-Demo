using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EncodeDemo
{
    public sealed class CyrillicEncodeProvider: IEncodeProvider
    {
        private const int SYNC_ENCODE_CHAR_LIMIT = 1;

        public CyrillicEncodeProvider()
        {
            generateEncodeTable();
        }
        
        #region Private members

        private Dictionary<char, char> encodeTable = new Dictionary<char, char>();
        private Random random = new Random();

        #endregion

        /// <summary>
        ///   Function to encode the queried character
        /// </summary>
        /// <param name="str">The character to encode</param>
        /// <returns>The encoded character from the input</returns>
        public string Encode(string str)
        {
            char[] encodeArr = new char[str.Length];
            char ch;

            for (int i = 0; i < str.Length; i++)
            {
                ch = str[i];
                if (EncodeService.IsAlphanumeric(ch) && encodeTable.ContainsKey(ch))
                {
                    encodeArr[i] = encodeTable[ch];
                }
                else
                {
                    encodeArr[i] = ch;
                }
            }

            return new string(encodeArr);
        }

        /// <summary>
        ///   Get the character count limit where we can accept the synch-encode mode.
        /// </summary>
        /// <returns>
        ///   The limit of the characters that we can use synchronized mode to encode
        /// </returns>
        public int GetSyncEncodeCharLimit()
        {
            return SYNC_ENCODE_CHAR_LIMIT;
        }

        /// <summary>
        ///   Reset and regenerate the encode table
        /// </summary>
        public async void RegenerateEncodeTable()
        {
            await Task.Run(() =>
            {
                encodeTable.Clear();
                generateEncodeTable();
            });
        }

        private void generateEncodeTable()
        {
            var uniqueSet = new HashSet<char>();
            char cyrillic;

            foreach (char ch in GetNextAlphanumeric())
            {
                do
                {
                    cyrillic = GetNextCyrillicChar();

                } while (uniqueSet.Contains(cyrillic));
                
                encodeTable.Add(ch, cyrillic);
                uniqueSet.Add(ch);
            }
        }

        private IEnumerable<char> GetNextAlphanumeric()
        {
            for (char ch = '0'; ch <= '9'; ch++)
            {
                yield return ch;
            }

            for (char ch = 'a'; ch <= 'z'; ch++)
            {
                yield return ch;
            }

            for (char ch = 'A'; ch <= 'Z'; ch++)
            {
                yield return ch;
            }
        }

        private char GetNextCyrillicChar()
        {
            return Convert.ToChar('\u0400' + random.Next(0, 255));
        }
    }
}
