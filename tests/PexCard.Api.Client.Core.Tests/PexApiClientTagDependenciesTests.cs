using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PexCard.Api.Client.Core.Enums;
using Xunit;

namespace PexCard.Api.Client.Core.Tests
{
    /// <summary>
    /// Tests for the business tag dependency list and its dependency type filter.
    /// </summary>
    public class PexApiClientTagDependenciesTests
    {
        /// <summary>
        /// Token passed to every call.
        /// </summary>
        private const string Token = "ext-token";

        /// <summary>
        /// Path of the business tag dependency list.
        /// </summary>
        private const string DependenciesPath = "/V4/tags/dependencies";

        /// <summary>
        /// An options dependency row as the API returns it.
        /// </summary>
        private const string OptionsRow =
            "{\"DependencyId\":\"dep-1\",\"DependentTagId\":\"tag-b\",\"Name\":\"Options\",\"Type\":\"Options\",\"DependsOnTagId\":\"tag-a\"," +
            "\"Created\":{\"UserId\":7,\"DateTime\":\"2026-11-01T10:00:00Z\"},\"Updated\":{\"UserId\":7,\"DateTime\":\"2026-11-01T10:00:00Z\"}}";

        /// <summary>
        /// A requirements dependency row as the API returns it, with fields this SDK version does not model.
        /// </summary>
        private const string RequirementsRow =
            "{\"DependencyId\":\"dep-2\",\"DependentTagId\":\"tag-b\",\"Name\":\"Required when\",\"Type\":\"Requirements\",\"DependsOnTagId\":\"tag-a\"," +
            "\"Rules\":[{\"RuleId\":\"r-1\",\"Condition\":{\"Type\":\"Select\",\"TriggerOptions\":[\"opt-1\"]},\"Required\":true}]," +
            "\"Default\":{\"Required\":false}," +
            "\"Created\":{\"UserId\":7,\"DateTime\":\"2026-11-01T10:00:00Z\"},\"Updated\":{\"UserId\":7,\"DateTime\":\"2026-11-01T10:00:00Z\"}}";

        /// <summary>
        /// The original overload sends no query string.
        /// </summary>
        [Fact]
        public async Task GetBusinessTagDependencies_WithoutTypes_SendsNoQuery()
        {
            Uri capturedUri = null;
            var client = CreateClient(new StubHandler(req =>
            {
                capturedUri = req.RequestUri;

                return OkDependencies(OptionsRow);
            }));

            var dependencies = await client.GetBusinessTagDependencies(Token);

            Assert.Equal(DependenciesPath, capturedUri.AbsolutePath);
            Assert.Equal(string.Empty, capturedUri.Query);
            Assert.Single(dependencies);
        }

        /// <summary>
        /// A null or empty types filter sends the same request as the original overload.
        /// </summary>
        /// <param name="useNull">
        /// <c>true</c> to pass <c>null</c>, <c>false</c> to pass an empty list.
        /// </param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetBusinessTagDependencies_WithNullOrEmptyTypes_SendsNoQuery(bool useNull)
        {
            Uri capturedUri = null;
            var client = CreateClient(new StubHandler(req =>
            {
                capturedUri = req.RequestUri;

                return OkDependencies(OptionsRow);
            }));

            await client.GetBusinessTagDependencies(Token, useNull ? null : Array.Empty<TagDependencyType>());

            Assert.Equal(DependenciesPath, capturedUri.AbsolutePath);
            Assert.Equal(string.Empty, capturedUri.Query);
        }

        /// <summary>
        /// One type is sent by name as a single <c>Types</c> parameter.
        /// </summary>
        /// <param name="type">
        /// The requested type.
        /// </param>
        /// <param name="expectedQuery">
        /// The expected query string without the leading question mark.
        /// </param>
        [Theory]
        [InlineData(TagDependencyType.Options, "Types=Options")]
        [InlineData(TagDependencyType.Requirements, "Types=Requirements")]
        public async Task GetBusinessTagDependencies_WithOneType_SendsItByName(TagDependencyType type, string expectedQuery)
        {
            Uri capturedUri = null;
            var client = CreateClient(new StubHandler(req =>
            {
                capturedUri = req.RequestUri;

                return OkDependencies(OptionsRow);
            }));

            await client.GetBusinessTagDependencies(Token, new[] { type });

            Assert.Equal(DependenciesPath, capturedUri.AbsolutePath);
            Assert.Equal("?" + expectedQuery, capturedUri.Query);
        }

        /// <summary>
        /// Two types are sent as a repeated <c>Types</c> parameter, duplicates once.
        /// </summary>
        [Fact]
        public async Task GetBusinessTagDependencies_WithTwoTypes_RepeatsTheParameter()
        {
            Uri capturedUri = null;
            var client = CreateClient(new StubHandler(req =>
            {
                capturedUri = req.RequestUri;

                return OkDependencies(OptionsRow, RequirementsRow);
            }));

            var dependencies = await client.GetBusinessTagDependencies(
                Token,
                new[] { TagDependencyType.Options, TagDependencyType.Requirements, TagDependencyType.Options });

            Assert.Equal(DependenciesPath, capturedUri.AbsolutePath);
            Assert.Equal("?Types=Options&Types=Requirements", capturedUri.Query);
            Assert.Equal(2, dependencies.Count);
        }

        /// <summary>
        /// A <c>Requirements</c> row in the list deserializes like an options row.
        /// </summary>
        [Fact]
        public async Task GetBusinessTagDependencies_RequirementsRow_Deserializes()
        {
            var client = CreateClient(new StubHandler(_ => OkDependencies(OptionsRow, RequirementsRow)));

            var dependencies = await client.GetBusinessTagDependencies(Token);

            Assert.Equal(2, dependencies.Count);
            var requirements = dependencies[1];
            Assert.Equal("dep-2", requirements.DependencyId);
            Assert.Equal("tag-b", requirements.DependentTagId);
            Assert.Equal("Requirements", requirements.Type);
            Assert.Equal(TagDependencyType.Requirements.ToString(), requirements.Type);
            Assert.Equal("tag-a", requirements.DependsOnTagId);
            Assert.Equal(7, requirements.Created.UserId);
        }

        /// <summary>
        /// An undefined type is rejected before any request is sent.
        /// </summary>
        [Fact]
        public async Task GetBusinessTagDependencies_WithUndefinedType_ThrowsBeforeSending()
        {
            var sent = false;
            var client = CreateClient(new StubHandler(_ =>
            {
                sent = true;

                return OkDependencies(OptionsRow);
            }));

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => client.GetBusinessTagDependencies(Token, new[] { TagDependencyType.Options, (TagDependencyType)42 }));
            Assert.False(sent);
        }

        #region private methods

        /// <summary>
        /// Creates a client that sends through the given handler.
        /// </summary>
        /// <param name="handler">
        /// The handler that answers the requests.
        /// </param>
        /// <returns>
        /// The client.
        /// </returns>
        private static PexApiClient CreateClient(HttpMessageHandler handler)
        {
            var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://coreapi.example/") };

            return new PexApiClient(httpClient);
        }

        /// <summary>
        /// Builds a 200 response holding the given dependency rows.
        /// </summary>
        /// <param name="rows">
        /// The JSON rows of the list.
        /// </param>
        /// <returns>
        /// The response.
        /// </returns>
        private static HttpResponseMessage OkDependencies(params string[] rows)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[" + string.Join(",", rows) + "]")
            };
        }

        /// <summary>
        /// Message handler that answers every request with a callback.
        /// </summary>
        private sealed class StubHandler : HttpMessageHandler
        {
            /// <summary>
            /// Produces the response for a request.
            /// </summary>
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

            /// <summary>
            /// Creates the handler.
            /// </summary>
            /// <param name="responder">
            /// Produces the response for a request.
            /// </param>
            public StubHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
            {
                _responder = responder;
            }

            /// <summary>
            /// Answers the request with the callback's response.
            /// </summary>
            /// <param name="request">
            /// The request.
            /// </param>
            /// <param name="cancellationToken">
            /// The cancellation token.
            /// </param>
            /// <returns>
            /// The response.
            /// </returns>
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(_responder(request));
            }
        }

        #endregion
    }
}
