using UnityEngine.Networking;

namespace SS.Utilities.Networking
{
    /// <summary>
    /// Appends a static bearer token authorization header to outgoing network streams 
    /// to authenticate authorized sessions against protected API endpoints.
    /// </summary>
    public class StandardAuthMiddleware : IRequestMiddleware
    {
        private readonly string _token;

        public StandardAuthMiddleware(string token)
        {
            _token = token;
        }

        public void OnSendingRequest(UnityWebRequest request, byte[] payload)
        {
            if (!string.IsNullOrEmpty(_token))
            {
                request.SetRequestHeader("Authorization", $"Bearer {_token}");
            }
        }
    }
}