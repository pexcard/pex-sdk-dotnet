using System;
using System.Security.Cryptography;
using System.Text;
using PexCard.Api.Client.Security;
using Xunit;

namespace PexCard.Api.Client.Core.Tests
{
    public class PexOAuthPkceTests
    {
        [Fact]
        public void CreateCodeVerifier_IsUrlSafe_AndRfcLength()
        {
            var verifier = PexOAuthPkce.CreateCodeVerifier();

            Assert.False(string.IsNullOrEmpty(verifier));
            Assert.InRange(verifier.Length, 43, 128); // RFC 7636 §4.1
            Assert.DoesNotContain('+', verifier);
            Assert.DoesNotContain('/', verifier);
            Assert.DoesNotContain('=', verifier);
        }

        [Fact]
        public void CreateCodeVerifier_IsRandomPerCall()
        {
            Assert.NotEqual(PexOAuthPkce.CreateCodeVerifier(), PexOAuthPkce.CreateCodeVerifier());
        }

        [Fact]
        public void CreateCodeChallenge_IsBase64UrlSha256_OfVerifier()
        {
            const string verifier = "test-verifier-1234567890-abcdefg";
            var expected = Base64UrlEncode(SHA256.HashData(Encoding.ASCII.GetBytes(verifier)));

            Assert.Equal(expected, PexOAuthPkce.CreateCodeChallenge(verifier));
        }

        [Fact]
        public void CreateCodeChallenge_IsDeterministic()
        {
            var verifier = PexOAuthPkce.CreateCodeVerifier();

            Assert.Equal(PexOAuthPkce.CreateCodeChallenge(verifier), PexOAuthPkce.CreateCodeChallenge(verifier));
        }

        [Fact]
        public void CreateCodeChallenge_IsUrlSafe()
        {
            var challenge = PexOAuthPkce.CreateCodeChallenge(PexOAuthPkce.CreateCodeVerifier());

            Assert.DoesNotContain('+', challenge);
            Assert.DoesNotContain('/', challenge);
            Assert.DoesNotContain('=', challenge);
        }

        [Fact]
        public void CreateCodeChallenge_Throws_OnEmpty()
        {
            Assert.Throws<ArgumentException>(() => PexOAuthPkce.CreateCodeChallenge(string.Empty));
        }

        [Fact]
        public void CreateState_IsUrlSafe_AndRandom()
        {
            var state1 = PexOAuthPkce.CreateState();
            var state2 = PexOAuthPkce.CreateState();

            Assert.False(string.IsNullOrEmpty(state1));
            Assert.NotEqual(state1, state2);
            Assert.DoesNotContain('+', state1);
            Assert.DoesNotContain('/', state1);
            Assert.DoesNotContain('=', state1);
        }

        [Fact]
        public void CodeChallengeMethod_IsS256()
        {
            Assert.Equal("S256", PexOAuthPkce.CodeChallengeMethod);
        }

        private static string Base64UrlEncode(byte[] bytes) =>
            Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }
}
