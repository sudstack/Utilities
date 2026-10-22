using System.Security.Cryptography;
using System.Text;
using UnityEngine.Networking;

namespace SS.Utilities.Networking
{
    /// <summary>
    /// Computes cryptographic HMAC-SHA256 signatures from request payloads and secret keys 
    /// to satisfy high-security transaction verification protocols demanded by payment gateways.
    /// </summary>
    public class PaymentSignatureMiddleware : IRequestMiddleware
    {
        private readonly string _secretKey;

        public PaymentSignatureMiddleware(string secretKey)
        {
            _secretKey = secretKey;
        }

        public void OnSendingRequest(UnityWebRequest request, byte[] payload)
        {
            if (payload == null || payload.Length == 0 || string.IsNullOrEmpty(_secretKey))
            {
                return;
            }

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey)))
            {
                byte[] hashBytes = hmac.ComputeHash(payload);

                // Converts the byte array to a lower-case hexadecimal string representation 
                // required by standard webhook verification headers.
                StringBuilder sb = new StringBuilder(hashBytes.Length * 2);
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                request.SetRequestHeader("X-VERIFY", sb.ToString());
            }
        }
    }
}