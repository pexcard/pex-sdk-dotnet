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
            Assert.Equal("OpenAi", model.Analysis.Platform);
            Assert.NotNull(model.Analysis.MatchCriteria?.Merchant);
            Assert.Equal("Acme", model.Analysis.MatchCriteria.Merchant.Value);
            Assert.Equal(0.95f, model.Analysis.MatchCriteria.Merchant.Confidence);
            Assert.Equal("MerchantName", model.Analysis.MatchCriteria.Merchant.Type);
            Assert.Equal("Match", model.Analysis.IntelligentMatch?.Status);
        }
    }
}
