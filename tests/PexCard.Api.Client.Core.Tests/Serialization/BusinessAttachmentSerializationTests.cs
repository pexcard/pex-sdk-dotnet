using Newtonsoft.Json;
using PexCard.Api.Client.Core.Enums;
using PexCard.Api.Client.Core.Models;
using Xunit;

namespace PexCard.Api.Client.Core.Tests.Serialization
{
    public class BusinessAttachmentSerializationTests
    {
        [Fact]
        public void CreateBusinessAttachmentRequest_SerializesEnumsAsStringNames()
        {
            var request = new CreateBusinessAttachmentRequestModel
            {
                Content = "SGVsbG8=",
                Type = AttachmentType.Pdf,
                UploadChannel = AttachmentUploadChannel.Email,
                Source = "sender@pexcard.com",
                FileName = "receipt.pdf"
            };

            var json = JsonConvert.SerializeObject(request);

            // Must serialize by NAME — the SDK AttachmentType (Image=0,Pdf=1) differs from the API's
            // underlying values, so an integer would map to the wrong type on the wire.
            Assert.Contains("\"Type\":\"Pdf\"", json);
            Assert.Contains("\"UploadChannel\":\"Email\"", json);
            Assert.DoesNotContain("\"Type\":1", json);
            Assert.DoesNotContain("\"UploadChannel\":0", json);
        }

        [Fact]
        public void CreateBusinessAttachmentRequest_OmitsUnsetOptionalFields()
        {
            var request = new CreateBusinessAttachmentRequestModel
            {
                Content = "SGVsbG8=",
                Type = AttachmentType.Image,
                UploadChannel = AttachmentUploadChannel.Sms,
                Source = "+15555550100"
            };

            var json = JsonConvert.SerializeObject(
                request,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            Assert.DoesNotContain("FileName", json);
            Assert.DoesNotContain("Note", json);
        }

        [Fact]
        public void CreateBusinessAttachmentRequest_SerializesNestedNote()
        {
            var request = new CreateBusinessAttachmentRequestModel
            {
                Content = "SGVsbG8=",
                Type = AttachmentType.Image,
                UploadChannel = AttachmentUploadChannel.Email,
                Source = "sender@pexcard.com",
                Note = new CreateBusinessAttachmentNoteModel
                {
                    Text = "Client lunch - please reimburse",
                    VisibleToCardholder = true,
                    IsSystemGenerated = false
                }
            };

            var json = JsonConvert.SerializeObject(request);

            Assert.Contains("\"Note\":{", json);
            Assert.Contains("\"Text\":\"Client lunch - please reimburse\"", json);
            Assert.Contains("\"VisibleToCardholder\":true", json);
            Assert.Contains("\"IsSystemGenerated\":false", json);
        }

        [Fact]
        public void CreateBusinessAttachment_DeserializesNewResponseFields()
        {
            const string json = "{\"AttachmentId\":\"att-1\",\"Type\":\"Image\",\"Size\":84213,\"MetadataId\":555," +
                "\"FileName\":\"Email-jane@company.com.jpg\",\"Pages\":1,\"DuplicateOfId\":null," +
                "\"UploadChannel\":\"Email\",\"Source\":\"jane@company.com\"}";

            var model = JsonConvert.DeserializeObject<CreateBusinessAttachmentModel>(json);

            Assert.Equal("Email-jane@company.com.jpg", model.FileName);
            Assert.Equal(1, model.Pages);
            Assert.Null(model.DuplicateOfId);
            Assert.Equal(AttachmentUploadChannel.Email, model.UploadChannel);
            Assert.Equal("jane@company.com", model.Source);
        }

        [Fact]
        public void CreateBusinessAttachment_DeserializesMetadataIdAndSelfLink()
        {
            const string json = "{\"AttachmentId\":\"att-1\",\"Type\":\"Pdf\",\"Size\":1234,\"MetadataId\":555,\"Link\":{\"self\":\"https://api.pexcard.com/Business/Attachments/555/att-1/Analysis\"}}";

            var model = JsonConvert.DeserializeObject<CreateBusinessAttachmentModel>(json);

            Assert.Equal("att-1", model.AttachmentId);
            Assert.Equal(AttachmentType.Pdf, model.Type);
            Assert.Equal(555, model.MetadataId);
            Assert.NotNull(model.Link);
            Assert.Equal("https://api.pexcard.com/Business/Attachments/555/att-1/Analysis", model.Link.Self);
        }

        [Fact]
        public void BusinessAttachmentAnalysis_DeserializesNestedGraph()
        {
            const string json = "{\"AttachmentId\":\"att-1\",\"MetadataId\":555,\"Analysis\":{" +
                "\"OperationId\":\"op-9\",\"Platform\":\"OpenAi\",\"ModelDeployment\":\"gpt-x\"," +
                "\"MatchCriteria\":{\"Merchant\":{\"Name\":\"Merchant\",\"Value\":\"Acme\",\"Confidence\":0.95,\"Type\":\"MerchantName\"}}," +
                "\"IntelligentMatch\":{\"OperationId\":\"op-9\",\"Status\":\"Match\",\"Reason\":\"matched on total+date\"}}}";

            var model = JsonConvert.DeserializeObject<BusinessAttachmentAnalysisModel>(json);

            Assert.Equal("att-1", model.AttachmentId);
            Assert.Equal(555, model.MetadataId);
            Assert.NotNull(model.Analysis);
            Assert.Equal("op-9", model.Analysis.OperationId);
            Assert.Equal(AttachmentAnalysisPlatform.OpenAi, model.Analysis.Platform);
            Assert.NotNull(model.Analysis.MatchCriteria?.Merchant);
            Assert.Equal("Acme", model.Analysis.MatchCriteria.Merchant.Value);
            Assert.Equal(0.95f, model.Analysis.MatchCriteria.Merchant.Confidence);
            Assert.Equal(MatchCriterionType.MerchantName, model.Analysis.MatchCriteria.Merchant.Type);
            Assert.Equal(IntelligentMatchStatus.Match, model.Analysis.IntelligentMatch?.Status);
        }

        [Fact]
        public void BusinessAttachment_DeserializesCommittedMatch()
        {
            const string json = "{\"AttachmentId\":\"att-1\",\"Type\":\"Image\",\"Size\":84213,\"MetadataId\":555," +
                "\"UploadStatus\":\"Loaded\",\"CreatedDateUtc\":\"2026-06-26T14:05:11Z\"," +
                "\"FileName\":\"receipt.png\",\"UploadChannel\":\"Email\",\"Match\":{" +
                "\"Status\":\"AutoMatch\",\"SuggestedMatchId\":\"sm-9\",\"NoMatchDateUtc\":null," +
                "\"CommitDateUtc\":\"2026-06-26T14:09:30Z\",\"MatchRetryCount\":0,\"Matched\":{" +
                "\"DateUtc\":\"2026-06-26T14:09:30Z\",\"By\":{\"AdminId\":null,\"UserId\":552201,\"PexUserId\":99812}}}}";

            var model = JsonConvert.DeserializeObject<BusinessAttachmentModel>(json);

            Assert.Equal("att-1", model.AttachmentId);
            Assert.Equal(AttachmentType.Image, model.Type);
            Assert.Equal(555, model.MetadataId);
            Assert.Equal(MetadataFileSaveState.Loaded, model.UploadStatus);
            Assert.Equal("receipt.png", model.FileName);
            Assert.Equal(AttachmentUploadChannel.Email, model.UploadChannel);

            Assert.NotNull(model.Match);
            Assert.Equal(AttachmentMatchStatus.AutoMatch, model.Match.Status);
            Assert.Equal("sm-9", model.Match.SuggestedMatchId);
            Assert.Null(model.Match.NoMatchDateUtc);
            Assert.NotNull(model.Match.CommitDateUtc);
            Assert.Equal(0, model.Match.MatchRetryCount);
            Assert.NotNull(model.Match.Matched);
            Assert.NotNull(model.Match.Matched.DateUtc);
            Assert.NotNull(model.Match.Matched.By);
            Assert.Equal(552201, model.Match.Matched.By.UserId);
            Assert.Equal(99812, model.Match.Matched.By.PexUserId);
        }

        [Fact]
        public void BusinessAttachment_DeserializesNullMatch()
        {
            const string json = "{\"AttachmentId\":\"att-1\",\"Type\":\"Pdf\",\"Size\":1234,\"MetadataId\":555," +
                "\"UploadStatus\":\"Loaded\",\"FileName\":\"invoice.pdf\",\"UploadChannel\":\"Sms\",\"Match\":null}";

            var model = JsonConvert.DeserializeObject<BusinessAttachmentModel>(json);

            Assert.Equal(AttachmentType.Pdf, model.Type);
            Assert.Equal(AttachmentUploadChannel.Sms, model.UploadChannel);
            Assert.Null(model.Match);
        }

        [Fact]
        public void BusinessAttachment_DeserializesNoMatchStatus()
        {
            const string json = "{\"AttachmentId\":\"att-1\",\"Type\":\"Pdf\",\"Size\":1234,\"MetadataId\":555," +
                "\"UploadChannel\":\"Email\",\"Match\":{\"Status\":\"NoMatch\"," +
                "\"NoMatchDateUtc\":\"2026-06-26T14:30:00Z\",\"MatchRetryCount\":1,\"Matched\":null}}";

            var model = JsonConvert.DeserializeObject<BusinessAttachmentModel>(json);

            Assert.NotNull(model.Match);
            Assert.Equal(AttachmentMatchStatus.NoMatch, model.Match.Status);
            Assert.NotNull(model.Match.NoMatchDateUtc);
            Assert.Null(model.Match.CommitDateUtc);
            Assert.Equal(1, model.Match.MatchRetryCount);
            Assert.Null(model.Match.Matched);
        }

        [Fact]
        public void MatchCriterion_DeserializesTypeAndStatusEnums()
        {
            const string json = "{\"AttachmentId\":\"att-1\",\"MetadataId\":555,\"Analysis\":{" +
                "\"Platform\":\"AzureDocumentAi\",\"MatchCriteria\":{" +
                "\"Total\":{\"Name\":\"Total\",\"Value\":\"42.00\",\"Confidence\":0.9,\"Type\":\"Total\"}}," +
                "\"IntelligentMatch\":{\"Status\":\"NoMatch\"}}}";

            var model = JsonConvert.DeserializeObject<BusinessAttachmentAnalysisModel>(json);

            Assert.Equal(AttachmentAnalysisPlatform.AzureDocumentAi, model.Analysis.Platform);
            Assert.Equal(MatchCriterionType.Total, model.Analysis.MatchCriteria.Total.Type);
            Assert.Equal(IntelligentMatchStatus.NoMatch, model.Analysis.IntelligentMatch.Status);
        }
    }
}
