namespace EncodeDemo
{
    /// <summary>
    /// The supported provider types that can be created by the factory
    /// </summary>
    public enum ProviderType
    {
        CyrillicEncoder
    }

    /// <summary>
    /// Create the encode provider object
    /// </summary>
    public static class ProviderFactory
    {
        /// <summary>
        /// Create the instance of the encode provider object
        /// </summary>
        /// <param name="type">The requested provider type to create by the factory</param>
        /// <returns>The instance of the desired encode provider</returns>
        public static IEncodeProvider CreateProvider(ProviderType type)
        {
            IEncodeProvider provider;

            switch (type)
            {
                case ProviderType.CyrillicEncoder:
                default:
                    provider = new CyrillicEncodeProvider();
                    break;
            }

            return provider;
        }
    }
}
