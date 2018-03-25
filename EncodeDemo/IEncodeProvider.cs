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
        /// <param name="ch">The character to encode</param>
        /// <returns>The encoded character from the input</returns>
        char Encode(char ch);
    }
}
