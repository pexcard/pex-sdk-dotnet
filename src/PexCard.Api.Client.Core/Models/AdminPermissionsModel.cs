namespace PexCard.Api.Client.Core.Models
{
    public class AdminPermissionsModel
    {
        public bool ViewAdministration { get; set; }

        public bool AddEditTerminateAdministrator { get; set; }

        public bool RequestDeleteACHTransfer { get; set; }

        public bool EditBusinessProfile { get; set; }

        public bool AddEditTerminateCard { get; set; }

        public bool CreateCardholder { get; set; }

        public bool ManageCardholder { get; set; }

        public bool? ViewCardUsage { get; set; }

        public bool RequestCardFunding { get; set; }

        public bool ModifyTransactionNotes { get; set; }
    }
}