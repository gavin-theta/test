using System;

namespace WelNetworks.BidWel.Portal.Core.Models
{
    public class DispatchInstructionDetail
    {

        public int WTPDispatchInstructionID { get; set; }
        public string DispatchGroup { get; set; }
        public string GroupDescription { get; set; }
        public string PrimaryDispatchTypeCode { get; set; }
        public string DispatchTypeDescription { get; set; }
        public string Node { get; set; }
        public string CorrelationId { get; set; }
        public int SequenceNumber { get; set; }
        public string DispatchEndpointOwner { get; set; }
        public bool IsUserResend { get; set; }
        public DateTime MessageSentTimeLocal { get; set; }
        public DateTime MessageSentTimeUTC { get; set; }
        public string Tradercode { get; set; }
        public float DispatchValue { get; set; }
        public DateTime DispatchTimeUTC { get; set; }
        public DateTime DispatchTimeLocal { get; set; }
        public DateTime DispatchIssueTimeUTC { get; set; }
        public DateTime DispatchIssueTimeLocal { get; set; }
        public DateTime AuditDateCreated { get; set; }
        public string AckType { get; set; }
        public DateTime AckAuditDateCreated { get; set; }

    }
}
