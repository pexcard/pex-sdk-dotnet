using Moq;
using PexCard.Api.Client.Core.Extensions;
using PexCard.Api.Client.Core.Models;
using System.Collections.Generic;
using Xunit;

namespace PexCard.Api.Client.Core.Tests.Extensions
{
    public class TransactionsListExtensionTests
    {
        private readonly Mock<IPexApiClient> _pexApiClient;

        public TransactionsListExtensionTests()
        {
            _pexApiClient = new Mock<IPexApiClient>();
        }

        [Fact]
        public void SelectCardAccountFees_ReturnsIsaFeeTransactions_GivenTransactionList()
        {
            //Arrange
            var transactions = new List<TransactionModel>
            {
                new TransactionModel { TransactionTypeCategory = "Spend", Description = "Purchase", }
            };

            var expectedFees = new List<TransactionModel>
            {
                new TransactionModel { TransactionTypeCategory = "CardAccountFee", Description = "ISA Fee", },
                new TransactionModel { TransactionTypeCategory = "CardAccountFee", Description = "ISA Fee Adjustment", },
            };
            transactions.AddRange(expectedFees);

            var chTransactions = new CardholderTransactions(transactions);

            //Act
            List<TransactionModel> actualFees = chTransactions.SelectCardAccountFees();

            //Assert
            Assert.Equal(expectedFees, actualFees);
        }
    }
}
