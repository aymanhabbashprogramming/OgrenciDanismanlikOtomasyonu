namespace EntityLayer
{
    public class clsSemesterReport
    {
        public int SemesterReportId { get; set; }
        public int AdvisorId { get; set; }
        public int SemesterId { get; set; }
        public string? SuccessSummary { get; set; }
        public string? SocialIssues { get; set; }
        public string? EconomicIssues { get; set; }
        public string? ActionsTaken { get; set; }
        public string? Suggestions { get; set; }
        public DateOnly? SubmittedAt { get; set; }
    }
}
