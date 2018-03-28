using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace EncodeDemo
{
    public static class EncodeService
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
        /// Update the encoded message based on the changes made to the base message. If not using partial updates (for better performance), just
        /// proide the provider and the current string to encode. 
        /// 
        /// Special cases 1: if provider is null, or current string is null, we will always return the current string.
        ///               2: if last encoded string is empty, or all update positions are using default values (0), then we will encode the entire 
        ///                  current string.
        ///               3: Remove in the encoded string will perform at the offset position first, then insert the encoded addition from the current
        ///                  string.
        ///                  
        /// </summary>
        /// <param name="provider">The encode provider</param>
        /// <param name="current">The new message string that carries updates</param>
        /// <param name="lastEncoded" default="">The encoded message to update</param>
        /// <param name="offset" default=0>The index of the position that an update has taken place</param>
        /// <param name="added" default=0>The amount of characters being added from the offset position</param>
        /// <param name="removed" default=0>The amount of characters being removed from the offset position</param>
        /// <returns>The updated encoded message</returns>
		public static string Encode(IEncodeProvider provider, string current, string lastEncoded = "", int offset = 0, int added = 0, int removed = 0)
		{
			SimulateSlowEncode(added);

            if (string.IsNullOrWhiteSpace(current) || provider == null)
            {
                // if a blank message or no provider, just use the input -- no need to encode
                return current;
            }

            if (string.IsNullOrEmpty(lastEncoded) || (offset == 0 && added == 0 && removed == 0))
            {
                // Case 1: if last message is empty, encode the entire string. note that if last is all white space,
                //         we could still optimize the encoding process to the updated parts, instead of going all 
                //         the white spaces (there could be 1M white spaces + 1 character update, avoid looping over
                //         all those white spaces is the key).
                // Case 2: if using all default values, consider this as a full update
                return provider.Encode(current);
            }

            try
            {
                // remove first
                if (removed > 0)
                {
                    if (removed == lastEncoded.Length)
                    {
                        lastEncoded = string.Empty;  // a full clear
                    }
                    else
                    {
                        lastEncoded = lastEncoded.Remove(offset, removed);
                    }
                }

                // added the encoded portion
                if (added > 0)
                {
                    var replace = provider.Encode(current.Substring(offset, added));
                    lastEncoded = lastEncoded.Insert(offset, replace);
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                // if giving offset completely out of range, this will happen.
                Console.WriteLine(string.Format(">> Trying to update the string at disallowed locations: {0}", e));
                Console.WriteLine(string.Format(">> Last message length: {0}; Current message length: {1}; Offset: {2}; Add: {3}; Remove: {4}", 
                    lastEncoded.Length, current.Length, offset, added, removed));

                // keep throwing 
                throw e;
            }

			return lastEncoded;
		}

        [Conditional("DEBUG")]
        private static void SimulateSlowEncode(int count)
        {
            // If the settings tell us to apply delay, then do it. 
            if (Properties.Settings.Default.ApplyAsyncDelay)
            {
                // Sleep 50 ms for each characters to update -- to simulate a very slow encoder
				Thread.Sleep(50 * (count + 1));
			}
        }
    }
}
