namespace SS.Utilities.Serialization
{
    /// <summary>
    /// Defines a standardized contract for text-based JSON serialization and deserialization routines, 
    /// enabling interchangeable formatting engines without tightly coupling dependent systems.
    /// </summary>
    public interface IJsonSerializer
    {
        /// <summary>
        /// Converts an object instance into its equivalent JSON string representation.
        /// </summary>
        string Serialize<T>(T target);

        /// <summary>
        /// Reconstructs a strongly-typed data object from a provided JSON payload string.
        /// </summary>
        T Deserialize<T>(string json);
    }
}