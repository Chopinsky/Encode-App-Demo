using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace EncodeDemo
{
    /// <summary>
    /// Encode provider to map English alphanumeric to random Cyrillic characters
    /// </summary>
    public sealed class CyrillicEncodeProvider: IEncodeProvider
    {
        public CyrillicEncodeProvider()
        {
            GenerateEncodeTable();
        }
        
        #region Private members

        private readonly Dictionary<char, char> encodeTable = new Dictionary<char, char>();
        private readonly Random random = new Random();
        private readonly RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();

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
        ///   Return the encode characters map from the provider
        /// </summary>
        /// <returns>The character -> character map used for the encoding</returns>
        public Dictionary<char, char> GetEncodeMap()
        {
            return encodeTable;
        }

        /// <summary>
        ///   Reset and regenerate the encode table
        /// </summary>
        public void RegenerateEncodeTable()
        {
            encodeTable.Clear();
            GenerateEncodeTable();         
        }

        private void GenerateEncodeTable()
        {
            char cyrillic;

            foreach (char ch in GetNextAlphanumeric())
            {
                do
                {
                    if (Properties.Settings.Default.UseSafeRandom)
                    {
                        // if use safe random, more expansive
                        cyrillic = GetNextCyrillicCharSafe();
                    }
                    else
                    {
                        // if use psudom random
                        cyrillic = GetNextCyrillicChar();
                    }

                } while (encodeTable.ContainsValue(cyrillic));
                
                encodeTable.Add(ch, cyrillic);
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
            // default to use standard Cyrillic character set
            return Convert.ToChar(0x0400 + random.Next(0, 255));
        }

        private char GetNextCyrillicCharSafe()
        {
            var bytes = new byte[1];
            provider.GetBytes(bytes);

            // default to use standard Cyrillic character set
            return Convert.ToChar(0x0400 + (bytes[0] & 0x00FF));
        }
    }
}
