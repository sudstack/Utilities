using System;
using UnityEngine;

namespace SS.Utilities.Networking
{
    [Serializable]
    public struct MockEndpointMapping
    {
        public string EndpointPattern;
        public TextAsset JsonPayload;
    }

    /// Maps network endpoint identifiers to static JSON text assets.
    /// Enables localized offline testing without deploying dedicated server infrastructure.
    [CreateAssetMenu(fileName = "MockDataRegistry", menuName = "SudStack/Utilities/Networking/Mock Data Registry")]
    public class MockDataRegistry : ScriptableObject
    {
        [SerializeField] private MockEndpointMapping[] _mockMappings;

        /// Retrieves the pre-defined JSON payload for a matching endpoint.
        /// Performs direct substring matching to bypass expensive dictionary hashing allocations.
        public string GetMockPayload(string requestUrl)
        {
            if (_mockMappings == null || string.IsNullOrEmpty(requestUrl)) return string.Empty;

            for (int i = 0; i < _mockMappings.Length; i++)
            {
                if (requestUrl.Contains(_mockMappings[i].EndpointPattern) && _mockMappings[i].JsonPayload != null)
                {
                    return _mockMappings[i].JsonPayload.text;
                }
            }

            Debug.LogWarning($"[MockDataRegistry] No mock JSON payload found for URL containing: {requestUrl}");
            return string.Empty;
        }
    }
}