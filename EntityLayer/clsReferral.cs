namespace EntityLayer
{
    public class clsReferral
    {
        public int ReferralId { get; set; }
        public int StudentId { get; set; }
        public int AdvisorId { get; set; }
        public string Unit { get; set; } = string.Empty;
        public DateOnly ReferredAt { get; set; }
        public string? Note { get; set; }
    }
}
