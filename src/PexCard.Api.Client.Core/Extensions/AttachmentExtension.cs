using System;
using PexCard.Api.Client.Core.Enums;
using PexCard.Api.Client.Core.Models;

namespace PexCard.Api.Client.Core.Extensions
{
    public static class AttachmentExtension
    {
        public static ReceiptModel GetReceipt(this AttachmentModel attachment)
        {
            var result = new ReceiptModel
            {
                Bytes = Convert.FromBase64String(attachment.Content),
                ContentType = GetMimeType(attachment),
                Name = GetName(attachment)
            };
            return result;
        }

        private static string GetMimeType(AttachmentModel attachment)
        {
            string result;
            if (attachment.Type.Equals(AttachmentType.Pdf))
            {
                result = "application/pdf";
            }
            else
            {
                var mimeStr = attachment.Content.Substring(0, 5);
                result = IsBase64Png(mimeStr) ? "image/png" : "image/jpeg";
            }
            return result;
        }

        private static string GetName(AttachmentModel attachment)
        {
            string result;
            if (attachment.Type.Equals(AttachmentType.Pdf))
            {
                result = $"{attachment.AttachmentId}.pdf";
            }
            else
            {
                var mimeStr = attachment.Content.Substring(0, 5);
                result = $"{attachment.AttachmentId}.{(IsBase64Png(mimeStr) ? "png" : "jpg")}";
            }
            return result;
        }

        private static bool IsBase64Png(string base64Str)
        {
            var result = base64Str.Equals("ivbor", StringComparison.InvariantCultureIgnoreCase);
            return result;
        }
    }
}
