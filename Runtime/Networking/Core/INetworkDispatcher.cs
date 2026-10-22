using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace SS.Utilities.Networking
{
    /// Defines a standardized protocol for executing asynchronous network transmissions.
    /// Facilitates swapping between live HTTP requests and local mock simulations without altering consumer logic.
    internal interface INetworkDispatcher
    {
        UniTask<Result<T>> SendAsync<T>(
            string url,
            string method,
            object payload,
            List<IRequestMiddleware> requestMiddlewares,
            CancellationToken cancellationToken);
    }
}