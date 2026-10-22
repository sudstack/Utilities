using UnityEngine;

namespace SS.Utilities.Serialization
{
    /// <summary>
    /// Implements JSON serialization routines leveraging Unity's native JsonUtility engine 
    /// to provide fast, allocation-conscious mapping for standard object structures.
    /// </summary>
    public class UnityJsonSerializer : IJsonSerializer
    {
        public string Serialize<T>(T target)
        {
            return JsonUtility.ToJson(target);
        }

        public T Deserialize<T>(string json)
        {
            return JsonUtility.FromJson<T>(json);
        }
    }
}