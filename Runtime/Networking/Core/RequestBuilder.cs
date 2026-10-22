using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.Networking;

namespace SS.Utilities.Networking
{
    /// <summary>
    /// Implements a fluent configuration pattern for constructing and dispatching 
    /// asynchronous network operations without triggering heap allocations during setup.
    /// </summary>
    public struct RequestBuilder<TResponse>
    {
        private readonly APIManager _apiManager;
        private readonly string _url;
        private readonly string _method;
        private object _payload;
        private readonly List<IRequestMiddleware> _middlewares;

        internal RequestBuilder(APIManager apiManager, string url, string method)
        {
            _apiManager = apiManager;
            _url = url;
            _method = method;
            _payload = null;
            _middlewares = new List<IRequestMiddleware>();
        }

        /// <summary>
        /// Attaches a strongly-typed payload object to be serialized into the request body stream.
        /// </summary>
        public RequestBuilder<TResponse> WithPayload<TPayload>(TPayload payload)
        {
            _payload = payload;
            return this;
        }

        /// <summary>
        /// Appends a request-specific middleware instance to execute custom interception routines prior to dispatch.
        /// </summary>
        public RequestBuilder<TResponse> WithMiddleware(IRequestMiddleware middleware)
        {
            if (middleware != null)
            {
                _middlewares.Add(middleware);
            }
            return this;
        }

        /// Appends a pre-formatted raw byte payload to the network stream.
        /// Bypasses the internal JSON serialization layer to optimize throughput for dynamically generated form data.
        public RequestBuilder<TResponse> WithRawBytes(byte[] rawPayloadBytes)
        {
            // The underlying dispatcher will recognize the byte array and skip the reflection-based serializer.
            _payload = rawPayloadBytes;
            return this;
        }

        /// Injects a pre-constructed JSON string directly into the payload pipeline.
        /// Eliminates structural serialization processing for manually formatted API requests.
        public RequestBuilder<TResponse> WithRawJson(string rawJson)
        {
            _payload = rawJson;
            return this;
        }
        /// <summary>
        /// Dispatches the configured network operation asynchronously, applying global and local middlewares 
        /// before processing deserialization outcomes.
        /// </summary>
        public async UniTask<Result<TResponse>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            return await _apiManager.SendAsync<TResponse>(_url, _method, _payload, _middlewares, cancellationToken);
        }
    }
}