namespace EntityLayer
{
    public class clsMeeting
    {
        public int MeetingId { get; set; }
        public int AdvisorId { get; set; }
        public string MeetingType { get; set; } = string.Empty;
        public DateTime MeetingAt { get; set; }
        public string? Subject { get; set; }
        public string? IssuesRaised { get; set; }
        public string? Notes { get; set; }
    }
}
