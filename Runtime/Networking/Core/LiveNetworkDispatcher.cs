using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using SS.Utilities.Serialization;

namespace SS.Utilities.Networking
{
    /// Orchestrates actual HTTP network communications via UnityWebRequest.
    /// Manages serialization layers and executes global middleware interception sequences prior to transmission.
    internal class LiveNetworkDispatcher : INetworkDispatcher
    {
        private readonly IJsonSerializer _serializer;
        private readonly List<IRequestMiddleware> _globalMiddlewares;

        public LiveNetworkDispatcher(IJsonSerializer serializer, IEnumerable<IRequestMiddleware> globalMiddlewares)
        {
            _serializer = serializer;
            _globalMiddlewares = globalMiddlewares != null ? new List<IRequestMiddleware>(globalMiddlewares) : new List<IRequestMiddleware>();
        }

        public async UniTask<Result<T>> SendAsync<T>(string url, string method, object payload, List<IRequestMiddleware> requestMiddlewares, CancellationToken cancellationToken)
        {
            byte[] payloadBytes = null;

            if (payload != null && (method == UnityWebRequest.kHttpVerbPOST || method == UnityWebRequest.kHttpVerbPUT))
            {
                // Evaluates payload type to determine the appropriate processing route.
                // Prevents redundant reflection-based serialization for already formatted data streams.
                if (payload is byte[] rawBytes)
                {
                    payloadBytes = rawBytes;
                }
                else if (payload is string rawJson)
                {
                    payloadBytes = Encoding.UTF8.GetBytes(rawJson);
                }
                else
                {
                    string json = _serializer.Serialize(payload);
                    payloadBytes = Encoding.UTF8.GetBytes(json);
                }
            }

            using (var request = new UnityWebRequest(url, method))
            {
                if (payloadBytes != null)
                {
                    request.uploadHandler = new UploadHandlerRaw(payloadBytes);
                    request.SetRequestHeader("Content-Type", "application/json");
                }
                request.downloadHandler = new DownloadHandlerBuffer();

                for (int i = 0; i < _globalMiddlewares.Count; i++)
                {
                    _globalMiddlewares[i].OnSendingRequest(request, payloadBytes);
                }

                if (requestMiddlewares != null)
                {
                    for (int i = 0; i < requestMiddlewares.Count; i++)
                    {
                        requestMiddlewares[i].OnSendingRequest(request, payloadBytes);
                    }
                }

                try
                {
                    await request.SendWebRequest().WithCancellation(cancellationToken);
                    long status = request.responseCode;
                    string responseText = request.downloadHandler?.text;

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        T data = default;
                        if (!string.IsNullOrEmpty(responseText) && typeof(T) != typeof(string))
                        {
                            data = _serializer.Deserialize<T>(responseText);
                        }
                        else if (typeof(T) == typeof(string))
                        {
                            data = (T)(object)responseText;
                        }
                        return new Result<T>(status, data, "Success");
                    }

                    return new Result<T>(status, default, request.error);
                }
                catch (OperationCanceledException)
                {
                    return new Result<T>(499, default, "Request Canceled");
                }
                catch (Exception ex)
                {
                    return new Result<T>(500, default, ex.Message);
                }
            }
        }
    }
}