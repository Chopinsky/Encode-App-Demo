namespace EncodeDemo
{
    /// <summary>
    /// Interface to implement for encode providers
    /// </summary>
    public interface IEncodeProvider
    {
        /// <summary>
        ///   Function to encode the queried character
        /// </summary>
        /// <param name="str">The string to encode</param>
        /// <returns>The encoded character from the input</returns>
        string Encode(string str);

        /// <summary>
        ///   Get the character count limit where we can accept the synch-encode mode.
        /// </summary>
        /// <returns>
        ///   The limit of the characters that we can use synchronized mode to encode
        /// </returns>
        int GetSyncEncodeCharLimit();

        /// <summary>
        ///   Reset and regenerate the encode table
        /// </summary>
        void RegenerateEncodeTable();
    }
}
