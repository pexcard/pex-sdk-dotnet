using System;
using System.Security.Cryptography;
using System.Text;

namespace PexCard.Api.Client.Security
{
    /// <summary>
    /// PKCE (RFC 7636) and <c>state</c> helpers for the OAuth 2.1 authorization-code flow. PEX
    /// requires the S256 code-challenge method.
    /// </summary>
    public static class PexOAuthPkce
    {
        public const string CodeChallengeMethod = "S256";

        private const int RandomByteLength = 32;

        /// <summary>
        /// Creates a high-entropy PKCE code verifier (base64url of 32 random bytes; 43 chars).
        /// </summary>
        public static string CreateCodeVerifier()
        {
            return Base64UrlEncode(GetRandomBytes());
        }

        /// <summary>
        /// Computes the S256 code challenge (base64url of SHA-256 of the verifier).
        /// </summary>
        public static string CreateCodeChallenge(string codeVerifier)
        {
            if (string.IsNullOrEmpty(codeVerifier))
            {
                throw new ArgumentException($"'{nameof(codeVerifier)}' cannot be null or empty.", nameof(codeVerifier));
            }

            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.ASCII.GetBytes(codeVerifier));
                return Base64UrlEncode(hash);
            }
        }

        /// <summary>
        /// Creates an opaque, high-entropy <c>state</c> value for CSRF protection / flow correlation.
        /// </summary>
        public static string CreateState()
        {
            return Base64UrlEncode(GetRandomBytes());
        }

        private static byte[] GetRandomBytes()
        {
            var bytes = new byte[RandomByteLength];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return bytes;
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }
}
