using System;

namespace WelNetworks.BidWel.Portal.Core.Models
{
    public class DispatchInstruction
    {

        public int WTPDispatchInstructionID { get; set; }
        public string PrimaryDispatchTypeCode { get; set; }
        public string DispatchTypeDescription { get; set; }
        public string DispatchGroup { get; set; }
        public string GroupDescription { get; set; }
        public string Node { get; set; }
        public string CorrelationId { get; set; }
        public int SequenceNumber { get; set; }
        public DateTime MessageSentTimeUTC { get; set; }
        public DateTime MessageSentTimeLocal { get; set; }
        public string TraderCode { get; set; }
        public float DispatchValue { get; set; }
        public DateTime DispatchTimeUTC { get; set; }
        public DateTime DispatchTimeLocal { get; set; }
        public DateTime DispatchIssueTimeUTC { get; set; }
        public DateTime DispatchIssueTimeLocal { get; set; }
        public string AckType { get; set; } = "";

    }
}
