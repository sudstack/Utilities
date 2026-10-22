using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using SS.Utilities.Serialization;

namespace SS.Utilities.Networking
{
    /// Evaluates the active environment configuration and routes network payload traffic.
    /// Implemented as a persistent scene entity to maintain transport state across application flow transitions.
    [DisallowMultipleComponent]
    public class APIManager : MonoBehaviour
    {
        public static APIManager Instance { get; private set; }

        [Header("Configurations")]
        [SerializeField] private ApiEnvironmentConfig _config;
        [SerializeField] private MockDataRegistry _mockRegistry;

        private INetworkDispatcher _liveDispatcher;
        private INetworkDispatcher _mockDispatcher;
        private readonly List<IRequestMiddleware> _globalMiddlewares = new List<IRequestMiddleware>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDispatchers();
        }

        private void InitializeDispatchers()
        {
            // Utilizes the native structural serializer to prevent heap allocation spikes associated with reflection-heavy third-party libraries.
            IJsonSerializer serializer = new UnityJsonSerializer();

            _liveDispatcher = new LiveNetworkDispatcher(serializer, _globalMiddlewares);
            _mockDispatcher = new MockNetworkDispatcher(_mockRegistry, serializer);
        }

        /// Appends interception modules into the global request pipeline dynamically.
        /// Enables external systems to inject authentication or signature headers after startup.
        public void RegisterMiddleware(IRequestMiddleware middleware)
        {
            if (middleware != null && !_globalMiddlewares.Contains(middleware))
            {
                _globalMiddlewares.Add(middleware);
            }
        }

        public RequestBuilder<T> Get<T>(string endpoint)
        {
            string fullUrl = CombineUrl(_config != null ? _config.GetBaseUrl() : string.Empty, endpoint);
            return new RequestBuilder<T>(this, fullUrl, UnityWebRequest.kHttpVerbGET);
        }

        public RequestBuilder<T> Post<T>(string endpoint)
        {
            string fullUrl = CombineUrl(_config != null ? _config.GetBaseUrl() : string.Empty, endpoint);
            return new RequestBuilder<T>(this, fullUrl, UnityWebRequest.kHttpVerbPOST);
        }

        internal async UniTask<Result<T>> SendAsync<T>(
            string url,
            string method,
            object payload,
            List<IRequestMiddleware> requestMiddlewares,
            CancellationToken cancellationToken)
        {
            if (_config != null && _config.CurrentEnvironment == ApiEnvironment.Mock)
            {
                return await _mockDispatcher.SendAsync<T>(url, method, payload, requestMiddlewares, cancellationToken);
            }

            return await _liveDispatcher.SendAsync<T>(url, method, payload, requestMiddlewares, cancellationToken);
        }

        private string CombineUrl(string baseUri, string endpoint)
        {
            if (string.IsNullOrEmpty(endpoint)) return baseUri ?? string.Empty;
            return $"{(baseUri ?? string.Empty).TrimEnd('/')}/{endpoint.TrimStart('/')}";
        }
    }
}