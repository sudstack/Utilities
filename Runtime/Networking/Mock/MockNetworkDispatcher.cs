using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using SS.Utilities.Serialization;

namespace SS.Utilities.Networking
{
    /// Simulates network latency and resolves offline JSON payloads.
    /// Intentionally bypasses middleware pipelines as offline testing environments do not require authorization headers or payload signatures.
    internal class MockNetworkDispatcher : INetworkDispatcher
    {
        private readonly MockDataRegistry _registry;
        private readonly IJsonSerializer _serializer;

        public MockNetworkDispatcher(MockDataRegistry registry, IJsonSerializer serializer)
        {
            _registry = registry;
            _serializer = serializer;
        }

        public async UniTask<Result<T>> SendAsync<T>(string url, string method, object payload, List<IRequestMiddleware> requestMiddlewares, CancellationToken cancellationToken)
        {
            try
            {
                // Binds the cancellation token to the delay task to prevent memory leaks if the user exits the context prematurely.
                await UniTask.Delay(1000, cancellationToken: cancellationToken);

                string jsonResponse = _registry != null ? _registry.GetMockPayload(url) : string.Empty;

                if (string.IsNullOrEmpty(jsonResponse))
                {
                    return new Result<T>(404, default, "Mock Payload Not Found");
                }

                T data = _serializer.Deserialize<T>(jsonResponse);
                return new Result<T>(200, data, "Mock Success");
            }
            catch (OperationCanceledException)
            {
                return new Result<T>(499, default, "Mock Request Canceled");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MockNetworkDispatcher] Failed to deserialize mock payload: {ex.Message}");
                return new Result<T>(500, default, "Mock Deserialization Error");
            }
        }
    }
}