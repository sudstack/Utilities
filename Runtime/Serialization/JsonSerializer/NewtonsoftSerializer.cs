using Newtonsoft.Json;

namespace SS.Utilities.Serialization
{
    /// <summary>
    /// Implements JSON serialization routines utilizing Newtonsoft.Json (Json.NET) 
    /// to handle complex, deeply nested, or polymorphic data structures that exceed native utility limitations.
    /// </summary>
    public class NewtonsoftSerializer : IJsonSerializer
    {
        private readonly JsonSerializerSettings _settings;

        /// <summary>
        /// Initializes a new instance with default serialization settings optimized for standard object mapping.
        /// </summary>
        public NewtonsoftSerializer()
        {
            _settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        /// <summary>
        /// Initializes a new instance with customized serialization configuration parameters.
        /// </summary>
        public NewtonsoftSerializer(JsonSerializerSettings settings)
        {
            _settings = settings;
        }

        public string Serialize<T>(T target)
        {
            return JsonConvert.SerializeObject(target, _settings);
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _settings);
        }
    }
}