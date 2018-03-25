using System.Collections.Generic;

namespace EncodeDemo
{
    public sealed class CyrillicEncodeProvider: IEncodeProvider
    {
        private static Dictionary<char, char> encodeMap = new Dictionary<char, char>();

        public CyrillicEncodeProvider()
        {
            //todo: populate the encode map
        }

        /// <summary>
        ///   Function to encode the queried character
        /// </summary>
        /// <param name="ch">The character to encode</param>
        /// <returns>The encoded character from the input</returns>
        public char Encode(char ch)
        {
            return ch;
        }
    }
}
