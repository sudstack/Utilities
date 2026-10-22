using UnityEngine.Networking;

namespace SS.Utilities.Networking
{
    /// <summary>
    /// Defines an interception contract to inspect or modify outgoing UnityWebRequest payloads 
    /// and headers prior to transmission, enabling decoupled security and logging pipelines.
    /// </summary>
    public interface IRequestMiddleware
    {
        /// <summary>
        /// Intercepts the active request instance to inject authorization parameters, 
        /// compute security signatures, or append diagnostic headers.
        /// </summary>
        void OnSendingRequest(UnityWebRequest request, byte[] payload);
    }
}