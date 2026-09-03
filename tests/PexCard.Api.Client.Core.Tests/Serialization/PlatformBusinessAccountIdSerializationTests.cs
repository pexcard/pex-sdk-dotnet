using Newtonsoft.Json;
using PexCard.Api.Client.Core.Models;
using Xunit;

namespace PexCard.Api.Client.Core.Tests.Serialization
{
    public class PlatformBusinessAccountIdSerializationTests
    {
        [Fact]
        public void BusinessDetails_ReadsPlatformBusinessAccountId()
        {
            const string json = "{\"BusinessAccountId\":5331803,\"PlatformBusinessAccountId\":812445}";

            var model = JsonConvert.DeserializeObject<BusinessDetailsModel>(json);

            Assert.Equal(5331803, model.BusinessAccountId);
            Assert.Equal(812445, model.PlatformBusinessAccountId);
        }

        [Fact]
        public void BusinessDetails_NullPlatformBusinessAccountIdStaysNull()
        {
            const string json = "{\"BusinessAccountId\":5331803,\"PlatformBusinessAccountId\":null}";

            var model = JsonConvert.DeserializeObject<BusinessDetailsModel>(json);

            // 0 would look like a real id and join to the wrong business downstream.
            Assert.Null(model.PlatformBusinessAccountId);
        }

        [Fact]
        public void BusinessDetails_MissingPlatformBusinessAccountIdStaysNull()
        {
            const string json = "{\"BusinessAccountId\":5331803}";

            var model = JsonConvert.DeserializeObject<BusinessDetailsModel>(json);

            Assert.Null(model.PlatformBusinessAccountId);
        }

        [Fact]
        public void TokenData_ReadsPlatformBusinessAccountId()
        {
            const string json = "{\"BusinessAccountId\":5331803,\"PlatformBusinessAccountId\":812445}";

            var model = JsonConvert.DeserializeObject<TokenDataModel>(json);

            Assert.Equal(5331803, model.BusinessAccountId);
            Assert.Equal(812445, model.PlatformBusinessAccountId);
        }

        [Fact]
        public void TokenData_NullPlatformBusinessAccountIdStaysNull()
        {
            const string json = "{\"BusinessAccountId\":5331803,\"PlatformBusinessAccountId\":null}";

            var model = JsonConvert.DeserializeObject<TokenDataModel>(json);

            Assert.Null(model.PlatformBusinessAccountId);
        }

        [Fact]
        public void TokenData_MissingPlatformBusinessAccountIdStaysNull()
        {
            const string json = "{\"BusinessAccountId\":5331803}";

            var model = JsonConvert.DeserializeObject<TokenDataModel>(json);

            Assert.Null(model.PlatformBusinessAccountId);
        }
    }
}
