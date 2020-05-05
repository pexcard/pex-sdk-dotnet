using Moq;
using Newtonsoft.Json;
using PexCard.Api.Client.Core.Enums;
using PexCard.Api.Client.Core.Exceptions;
using PexCard.Api.Client.Core.Extensions;
using PexCard.Api.Client.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Xunit;
using Xunit.Sdk;

namespace PexCard.Api.Client.Core.Tests.Extensions
{
    public class IPexApiClientExtensionsTests
    {
        private readonly Mock<IPexApiClient> _mockPexApiClient;

        public IPexApiClientExtensionsTests()
        {
            _mockPexApiClient = new Mock<IPexApiClient>();
        }

        [Fact]
        public async void GetTagAllocations_Given1TagAllocated4Ways_Returns4MatchingAllocations()
        {
            //Arrange
            string allocationTagId = "5e95c111aa538301d0062869";
            string dropdownTagId = "5de64845aa53810dc4b3541b";

            var tagDetails = new List<TagDetailsModel>
            {
                new TagDetailsModel
                {
                    Id = dropdownTagId,
                    Type = CustomFieldType.Dropdown,
                },
                new TagDetailsModel
                {
                    Id = allocationTagId,
                    Type = CustomFieldType.Allocation,
                },
            };
            _mockPexApiClient.Setup(api => api.GetTags(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(tagDetails);

            decimal transactionAmount = -2.01M;
            var transactions = new List<TransactionModel>
            {
                new TransactionModel
                {
                    TransactionId = 419086499,
                    TransactionAmount = transactionAmount,
                    TransactionTags = new TransactionTagsModel
                    {
                        Tags = new List<TransactionTagModel>
                        {
                            new TransactionTagModel
                            {
                                FieldId = allocationTagId,
                                Value = $"{{\"TagId\":\"{allocationTagId}\",\"Allocation\":[{{\"TagId\":\"{dropdownTagId}\",\"Value\":\"10\"}}],\"Amount\":\"0.50\"}}",
                                Name = "Allocation_6215",
                            },
                            new TransactionTagModel
                            {
                                FieldId = allocationTagId,
                                Value = $"{{\"TagId\":\"{allocationTagId}\",\"Allocation\":[{{\"TagId\":\"{dropdownTagId}\",\"Value\":\"55\"}}],\"Amount\":\"0.50\"}}",
                                Name = "Allocation_6215",
                            },
                            new TransactionTagModel
                            {
                                FieldId = allocationTagId,
                                Value = $"{{\"TagId\":\"{allocationTagId}\",\"Allocation\":[{{\"TagId\":\"{dropdownTagId}\",\"Value\":\"10\"}}],\"Amount\":\"0.50\"}}",
                                Name = "Allocation_6215",
                            },
                            new TransactionTagModel
                            {
                                FieldId = allocationTagId,
                                Value = $"{{\"TagId\":\"{allocationTagId}\",\"Allocation\":[{{\"TagId\":\"{dropdownTagId}\",\"Value\":\"7\"}}],\"Amount\":\"0.51\"}}",
                                Name = "Allocation_6215",
                            },
                        },
                        State = FieldsState.Selected,
                        FieldsVersion = "5e9dbf59aa53831e5050a927",
                    },
                }
            };

            //var chTransactions = new CardholderTransactions(transactions);

            //Act
            Dictionary<long, List<AllocationTagValue>> actualAllocations = await _mockPexApiClient.Object.GetTagAllocations("d5fd5a3ba8324c1589c02d5414cb4358", transactions);

            //Assert
            Assert.NotNull(actualAllocations);
            Assert.Equal(transactions.Count, actualAllocations.Count);

            foreach (TransactionModel transaction in transactions)
            {
                Assert.NotNull(transaction);
                Assert.True(actualAllocations.ContainsKey(transaction.TransactionId));

                if (!actualAllocations.TryGetValue(transaction.TransactionId, out List<AllocationTagValue> actualAllocation))
                {
                    throw new XunitException($"Could not find allocation for {nameof(transaction.TransactionId)} '{transaction.TransactionId}'.");
                }

                Assert.NotNull(actualAllocation);

                Assert.NotNull(transaction.TransactionTags);
                Assert.NotNull(transaction.TransactionTags.Tags);
                Assert.Equal(4, actualAllocation.Count);

                var expectedAllocation = new List<AllocationTagValue>();
                foreach (TransactionTagModel tag in transaction.TransactionTags.Tags)
                {
                    Assert.NotNull(tag.Value); //"{\"TagId\":\"5e95c111aa538301d0062869\",\"Allocation\":[{\"TagId\":\"5de64845aa53810dc4b3541b\",\"Value\":\"10\"}],\"Amount\":\"0.50\"}"
                    var expectedAllocationValue = JsonConvert.DeserializeObject<AllocationTagValue>(tag.Value.ToString());

                    Assert.NotNull(expectedAllocationValue);
                    Assert.NotNull(expectedAllocationValue.Allocation);

                    expectedAllocation.Add(expectedAllocationValue);
                }

                foreach (AllocationTagValue expectedAllocationValue in expectedAllocation)
                {
                    //For each actualAllocation, find a matching expectedAllocation.
                    Assert.Contains(
                        actualAllocation, 
                        aav => aav.TagId == expectedAllocationValue.TagId && aav.Amount == expectedAllocationValue.Amount && aav.Allocation.All(aaa => expectedAllocationValue.Allocation.Any(eaa => eaa.TagId == aaa.TagId && eaa.Value.Equals(aaa.Value))));
                }

                foreach (AllocationTagValue actualAllocationValue in actualAllocation)
                {
                    //For each actualAllocation, find a matching expectedAllocation.
                    Assert.Contains(
                        expectedAllocation, 
                        eav => eav.TagId == actualAllocationValue.TagId && eav.Amount == actualAllocationValue.Amount && eav.Allocation.All(eaa => actualAllocationValue.Allocation.Any(aaa => aaa.TagId == eaa.TagId && aaa.Value.Equals(eaa.Value))));

                    transactionAmount += actualAllocationValue.Amount;
                }

                Assert.Equal(0, transactionAmount);

                //Opted to not go with the collection assert to make debugging easier.
                //Assert.Equal(expectedAllocation, actualAllocation);
            }
        }

        [Fact]
        public async void GetTagAllocations_Given4TagsAllocated1Way_Returns1Allocation()
        {
            //Arrange
            string dropdownTagId1 = "5e01e1e8aa53821e006da8ff";
            string dropdownTagId2 = "5e01e1fcaa53821e006da901";
            string dropdownTagId3 = "5e01e209aa53821e006da903";
            string dropdownTagId4 = "5de64845aa53810dc4b3541b";

            var tagDetails = new List<TagDetailsModel>
            {
                new TagDetailsModel
                {
                    Id = dropdownTagId1,
                    Type = CustomFieldType.Dropdown,
                },
                new TagDetailsModel
                {
                    Id = dropdownTagId2,
                    Type = CustomFieldType.Dropdown,
                },
                new TagDetailsModel
                {
                    Id = dropdownTagId3,
                    Type = CustomFieldType.Dropdown,
                },
                new TagDetailsModel
                {
                    Id = dropdownTagId4,
                    Type = CustomFieldType.Dropdown,
                },
            };
            _mockPexApiClient.Setup(api => api.GetTags(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(tagDetails);

            var transactions = new List<TransactionModel>
            {
                new TransactionModel
                {
                    TransactionId = 419086499,
                    TransactionAmount = -3.50M,
                    TransactionTags = new TransactionTagsModel
                    {
                        Tags = new List<TransactionTagModel>
                        {
                            new TransactionTagModel
                            {
                                FieldId = dropdownTagId1,
                                Value = $"225756",
                                Name = "Aplos Fund",
                            },
                            new TransactionTagModel
                            {
                                FieldId = dropdownTagId2,
                                Value = $"5708502",
                                Name = "Aplos Contact",
                            },
                            new TransactionTagModel
                            {
                                FieldId = dropdownTagId3,
                                Value = $"5001",
                                Name = "Aplos Account",
                            },
                            new TransactionTagModel
                            {
                                FieldId = dropdownTagId4,
                                Value = $"Equipment Rental",
                                Name = "QBO Expense Category",
                            },
                        },
                        State = FieldsState.Selected,
                        FieldsVersion = "5e95c13baa53822810081e56",
                    },
                }
            };

            //var chTransactions = new CardholderTransactions(transactions);

            //Act
            Dictionary<long, List<AllocationTagValue>> actualAllocations = await _mockPexApiClient.Object.GetTagAllocations("d5fd5a3ba8324c1589c02d5414cb4358", transactions);

            //Assert
            Assert.NotNull(actualAllocations);
            Assert.Equal(transactions.Count, actualAllocations.Count);

            foreach (TransactionModel transaction in transactions)
            {
                Assert.NotNull(transaction);
                Assert.True(actualAllocations.ContainsKey(transaction.TransactionId));

                if (!actualAllocations.TryGetValue(transaction.TransactionId, out List<AllocationTagValue> actualAllocation))
                {
                    throw new XunitException($"Could not find allocation for {nameof(transaction.TransactionId)} '{transaction.TransactionId}'.");
                }

                Assert.NotNull(actualAllocation);

                Assert.NotNull(transaction.TransactionTags);
                Assert.NotNull(transaction.TransactionTags.Tags);
                Assert.Single(actualAllocation);

                AllocationTagValue actualDefaultAllocation = actualAllocation.Single();
                Assert.NotNull(actualDefaultAllocation);
                Assert.Null(actualDefaultAllocation.TagId);
                Assert.NotEmpty(actualDefaultAllocation.Allocation);
                Assert.Equal(transaction.TransactionAmount, -actualDefaultAllocation.Amount);


                var expectedAllocation = new List<AllocationTagValue>();

                var expectedAllocationValues = new List<TagValueItem>();
                foreach (TransactionTagModel tag in transaction.TransactionTags.Tags)
                {
                    expectedAllocationValues.Add(new TagValueItem { TagId = tag.FieldId, Value = tag.Value, });
                }

                expectedAllocation.Add(new AllocationTagValue { TagId = null, Allocation = expectedAllocationValues, Amount = -transaction.TransactionAmount, });

                foreach (AllocationTagValue expectedAllocationValue in expectedAllocation)
                {
                    //For each expectedAllocation, find a matching actualAllocation.
                    Assert.Contains(actualAllocation, aav => aav.TagId == expectedAllocationValue.TagId && aav.Amount == expectedAllocationValue.Amount && aav.Allocation.All(aaa => expectedAllocationValue.Allocation.Any(eaa => eaa.TagId == aaa.TagId && eaa.Value.Equals(aaa.Value))));
                }

                foreach (AllocationTagValue actualAllocationValue in actualAllocation)
                {
                    //For each actualAllocation, find a matching expectedAllocation.
                    Assert.Contains(expectedAllocation, eav => eav.TagId == actualAllocationValue.TagId && eav.Amount == actualAllocationValue.Amount && eav.Allocation.All(eaa => actualAllocationValue.Allocation.Any(aaa => aaa.TagId == eaa.TagId && aaa.Value.Equals(eaa.Value))));
                }

                //Opted to not go with the collection assert to make debugging easier.
                //Assert.Equal(expectedAllocation, actualAllocation);
            }
        }

        [Fact]
        public async void GetTagAllocations_Given4TagsAllocated1WayButTagsDisabled_PexApiClientException()
        {
            //Arrange
            string dropdownTagId1 = "5e01e1e8aa53821e006da8ff";
            string dropdownTagId2 = "5e01e1fcaa53821e006da901";
            string dropdownTagId3 = "5e01e209aa53821e006da903";
            string dropdownTagId4 = "5de64845aa53810dc4b3541b";
            _mockPexApiClient.Setup(api => api.GetTags(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new PexApiClientException(HttpStatusCode.Forbidden, "This business does not support the Tags Feature"));

            var transactions = new List<TransactionModel>
            {
                new TransactionModel
                {
                    TransactionId = 419086499,
                    TransactionAmount = -3.50M,
                    TransactionTags = new TransactionTagsModel
                    {
                        Tags = new List<TransactionTagModel>
                        {
                            new TransactionTagModel
                            {
                                FieldId = dropdownTagId1,
                                Value = $"225756",
                                Name = "Aplos Fund",
                            },
                            new TransactionTagModel
                            {
                                FieldId = dropdownTagId2,
                                Value = $"5708502",
                                Name = "Aplos Contact",
                            },
                            new TransactionTagModel
                            {
                                FieldId = dropdownTagId3,
                                Value = $"5001",
                                Name = "Aplos Account",
                            },
                            new TransactionTagModel
                            {
                                FieldId = dropdownTagId4,
                                Value = $"Equipment Rental",
                                Name = "QBO Expense Category",
                            },
                        },
                        State = FieldsState.Selected,
                        FieldsVersion = "5e95c13baa53822810081e56",
                    },
                }
            };

            //Act / Assert
            await Assert.ThrowsAsync<PexApiClientException>(() => _mockPexApiClient.Object.GetTagAllocations("d5fd5a3ba8324c1589c02d5414cb4358", transactions));
        }

        [Fact]
        public async void GetTagAllocations_GivenNullTransactionTags_Returns1MatchingAllocation()
        {
            //Arrange
            _mockPexApiClient.Setup(api => api.GetTags(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new PexApiClientException(HttpStatusCode.Forbidden, "This business does not support the Tags Feature"));

            var transactions = new List<TransactionModel>
            {
                new TransactionModel
                {
                    TransactionId = 419086499,
                    TransactionAmount = -3.50M,
                }
            };

            //Act
            Dictionary<long, List<AllocationTagValue>> actualAllocations = await _mockPexApiClient.Object.GetTagAllocations("d5fd5a3ba8324c1589c02d5414cb4358", transactions);

            //Assert
            Assert.NotNull(actualAllocations);
            Assert.Equal(transactions.Count, actualAllocations.Count);

            foreach (TransactionModel transaction in transactions)
            {
                Assert.NotNull(transaction);
                Assert.True(actualAllocations.ContainsKey(transaction.TransactionId));

                if (!actualAllocations.TryGetValue(transaction.TransactionId, out List<AllocationTagValue> actualAllocation))
                {
                    throw new XunitException($"Could not find allocation for {nameof(transaction.TransactionId)} '{transaction.TransactionId}'.");
                }

                Assert.NotNull(actualAllocation);

                Assert.Single(actualAllocation);

                AllocationTagValue actualDefaultAllocation = actualAllocation.Single();
                Assert.NotNull(actualDefaultAllocation);
                Assert.Null(actualDefaultAllocation.TagId);
                Assert.Empty(actualDefaultAllocation.Allocation);
                Assert.Equal(transaction.TransactionAmount, -actualDefaultAllocation.Amount);
            }
        }
    }
}
