using System;

namespace PexCard.Api.Client.Core.Models
{
    /// <summary>
    /// AI analysis details for a business source attachment.
    /// </summary>
    public class AttachmentAnalysisDetailsModel
    {
        /// <summary>Analysis operation identifier.</summary>
        public string OperationId { get; set; }

        /// <summary>Analysis platform (e.g. AwsTextract, AzureDocumentAi, OpenAi, AzureOpenAi).</summary>
        public string Platform { get; set; }

        /// <summary>When the analysis record was created (UTC).</summary>
        public DateTime CreatedDateUtc { get; set; }

        /// <summary>When analysis started (UTC).</summary>
        public DateTime? StartAnalysisDateUtc { get; set; }

        /// <summary>When analysis finished (UTC).</summary>
        public DateTime? FinishAnalysisDateUtc { get; set; }

        /// <summary>Extracted/matched field criteria.</summary>
        public AttachmentMatchCriteriaModel MatchCriteria { get; set; }

        /// <summary>Model deployment used for analysis.</summary>
        public string ModelDeployment { get; set; }

        /// <summary>Intelligent (AI) match outcome.</summary>
        public AttachmentIntelligentMatchModel IntelligentMatch { get; set; }
    }
}
