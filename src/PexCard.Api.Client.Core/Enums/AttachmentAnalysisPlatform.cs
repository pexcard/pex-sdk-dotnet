using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PexCard.Api.Client.Core.Enums
{
    /// <summary>
    /// AI platform used to analyze a business source attachment.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AttachmentAnalysisPlatform
    {
        /// <summary>Platform not specified or not recognized.</summary>
        Unknown,

        /// <summary>AWS Textract.</summary>
        AwsTextract,

        /// <summary>Azure Document AI.</summary>
        AzureDocumentAi,

        /// <summary>OpenAI.</summary>
        OpenAi,

        /// <summary>Azure OpenAI.</summary>
        AzureOpenAi
    }
}
