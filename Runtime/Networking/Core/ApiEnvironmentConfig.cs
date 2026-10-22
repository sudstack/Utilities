using System;
using UnityEngine;

namespace SS.Utilities.Networking
{
    public enum ApiEnvironment
    {
        Development,
        Staging,
        Production,
        Mock
    }

    [Serializable]
    public struct EnvironmentEndpoint
    {
        public ApiEnvironment Environment;
        public string BaseUrl;
    }

    /// Stores environment-specific configuration parameters.
    /// Decouples deployment endpoints from application logic via Unity's ScriptableObject asset pipeline.
    [CreateAssetMenu(fileName = "ApiEnvironmentConfig", menuName = "SudStack/Utilities/Networking/Api Environment Config")]
    public class ApiEnvironmentConfig : ScriptableObject
    {
        [SerializeField] private ApiEnvironment _currentEnvironment = ApiEnvironment.Development;
        [SerializeField] private EnvironmentEndpoint[] _endpoints;

        public ApiEnvironment CurrentEnvironment => _currentEnvironment;

        /// Resolves the corresponding base URL string for the active deployment environment.
        /// Utilizes allocation-free array iteration routines to prevent heap garbage.
        public string GetBaseUrl()
        {
            if (_endpoints == null) return string.Empty;
            for (int i = 0; i < _endpoints.Length; i++)
            {
                if (_endpoints[i].Environment == _currentEnvironment)
                {
                    return _endpoints[i].BaseUrl;
                }
            }
            return string.Empty;
        }
    }
}