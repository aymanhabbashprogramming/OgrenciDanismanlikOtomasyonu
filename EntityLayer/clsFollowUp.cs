namespace EntityLayer
{
    public class clsFollowUp
    {
        public int FollowUpId { get; set; }
        public int StudentId { get; set; }
        public int AdvisorId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateOnly FlaggedAt { get; set; }
        public DateOnly? ClearedAt { get; set; }
    }
}
