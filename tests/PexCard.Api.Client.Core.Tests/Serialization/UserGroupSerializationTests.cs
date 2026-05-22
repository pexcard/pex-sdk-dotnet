using System.Collections.Generic;
using Newtonsoft.Json;
using PexCard.Api.Client.Core.Models;
using Xunit;

namespace PexCard.Api.Client.Core.Tests.Serialization
{
    public class UserGroupSerializationTests
    {
        [Fact]
        public void UserGroupBrief_DeserializesFromServer()
        {
            const string json = @"{
                ""UserGroupId"": 1234567890123,
                ""Name"": ""Finance"",
                ""GroupType"": ""CardholderGroup"",
                ""LegacyGroupId"": 42
            }";

            var model = JsonConvert.DeserializeObject<UserGroupBrief>(json);

            Assert.Equal(1234567890123L, model.UserGroupId);
            Assert.Equal("Finance", model.Name);
            Assert.Equal("CardholderGroup", model.GroupType);
            Assert.Equal(42, model.LegacyGroupId);
        }

        [Fact]
        public void UserGroupBrief_AllowsNullLegacyGroupId()
        {
            const string json = @"{
                ""UserGroupId"": 7,
                ""Name"": ""Ops"",
                ""GroupType"": ""CardholderGroup"",
                ""LegacyGroupId"": null
            }";

            var model = JsonConvert.DeserializeObject<UserGroupBrief>(json);

            Assert.Null(model.LegacyGroupId);
        }

        [Fact]
        public void UserGroupCardholder_DeserializesFromServer()
        {
            const string json = @"{
                ""AccountId"": 555,
                ""FirstName"": ""Ada"",
                ""LastName"": ""Lovelace"",
                ""UserName"": ""ada@example.com""
            }";

            var model = JsonConvert.DeserializeObject<UserGroupCardholder>(json);

            Assert.Equal(555, model.AccountId);
            Assert.Equal("Ada", model.FirstName);
            Assert.Equal("Lovelace", model.LastName);
            Assert.Equal("ada@example.com", model.UserName);
        }

        [Fact]
        public void CardholderProfile_CarriesUserGroupAlongsideLegacyGroup()
        {
            const string json = @"{
                ""AccountId"": 10,
                ""CardholderGroupId"": 42,
                ""UserGroupId"": 1234567890123,
                ""UserGroups"": [
                    { ""UserGroupId"": 1234567890123, ""Name"": ""Finance"", ""GroupType"": ""CardholderGroup"", ""LegacyGroupId"": 42 }
                ]
            }";

            var model = JsonConvert.DeserializeObject<CardholderProfileModel>(json);

#pragma warning disable CS0618 // legacy field intentionally exercised
            Assert.Equal(42, model.CardholderGroupId);
#pragma warning restore CS0618
            Assert.Equal(1234567890123L, model.UserGroupId);
            Assert.Single(model.UserGroups);
            Assert.Equal("Finance", model.UserGroups[0].Name);
        }

        [Fact]
        public void CreateUserGroupRequest_SerializesNameAsPascalCase()
        {
            var json = JsonConvert.SerializeObject(new PexCard.Api.Client.Models.CreateUserGroupRequest { Name = "Finance" });

            Assert.Equal(@"{""Name"":""Finance""}", json);
        }

        [Fact]
        public void BusinessDetails_CarriesUserGroupsList()
        {
            const string json = @"{
                ""BusinessAccountId"": 1,
                ""UserGroups"": [
                    { ""UserGroupId"": 9, ""Name"": ""Travel"", ""GroupType"": ""CardholderGroup"", ""LegacyGroupId"": null }
                ]
            }";

            var model = JsonConvert.DeserializeObject<BusinessDetailsModel>(json);

            Assert.NotNull(model.UserGroups);
            Assert.Single(model.UserGroups);
            Assert.Equal(9, model.UserGroups[0].UserGroupId);
        }
    }
}
