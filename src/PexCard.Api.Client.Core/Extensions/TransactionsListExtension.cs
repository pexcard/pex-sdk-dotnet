using System;
using System.Collections.Generic;
using System.Linq;
using PexCard.Api.Client.Core.Enums;
using PexCard.Api.Client.Core.Models;

namespace PexCard.Api.Client.Core.Extensions
{
    public static class TransactionsListExtension
    {
        private const string BusinessFundingCategory = "BusinessFunding";
        private const string CardAccountFeeCategory = "CardAccountFee";
        private const string BusinessAccountFeeCategory = "BusinessAccountFee";
        private const string DisputeCategory = "Dispute";
        private const string MetadataApprovalStatusApproved = "Approved";

        public static List<TransactionModel> SelectTransactionsToSync(this CardholderTransactions transactions,
            bool approvedOnly, string syncedNote = "")
        {
            var result = transactions?.Where(t =>
                    (t.TransactionType == TransactionType.Network || (t.TransactionType == TransactionType.Transfer && t.TransactionTypeCategory == DisputeCategory)) &&
                    (string.IsNullOrEmpty(syncedNote) ||
                     !(t.TransactionNotes?.Exists(n => n.NoteText?.ToLower().Contains(syncedNote?.ToLower()) == true) ?? false)) &&
                    (!approvedOnly || string.Equals(t.MetadataApprovalStatus, MetadataApprovalStatusApproved, StringComparison.InvariantCultureIgnoreCase)))
                .ToList();

            return result ?? new List<TransactionModel>();
        }

        public static List<TransactionModel> SelectCardAccountFees(this CardholderTransactions transactions)
        {
            var result = transactions?.Where(t => string.Equals(t.TransactionTypeCategory, CardAccountFeeCategory, StringComparison.InvariantCultureIgnoreCase)).ToList();

            return result;
        }

        public static List<TransactionModel> SelectBusinessAccountFees(this BusinessAccountTransactions transactions)
        {
            var result = transactions?.Where(t => string.Equals(t.TransactionTypeCategory, BusinessAccountFeeCategory, StringComparison.InvariantCultureIgnoreCase)).ToList();

            return result;
        }

        public static List<TransactionModel> SelectBusinessAccountTransfers(this BusinessAccountTransactions transactions)
        {
            var result = transactions?.Where(t => string.Equals(t.TransactionTypeCategory, BusinessFundingCategory, StringComparison.InvariantCultureIgnoreCase)).ToList();

            return result;
        }
    }
}
