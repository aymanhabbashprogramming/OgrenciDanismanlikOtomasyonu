namespace EntityLayer
{
    public class clsRequest
    {
        public int RequestId { get; set; }
        public int StudentId { get; set; }
        public int RequestTypeId { get; set; }
        public int AdvisorId { get; set; }
        public int SemesterId { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string? Reason { get; set; }
        public decimal? DeclaredGano { get; set; }
        public string? Excuse { get; set; }
        public string? ContactAddress { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime? DecidedAt { get; set; }
    }
}
