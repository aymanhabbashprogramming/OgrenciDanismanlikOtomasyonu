namespace EntityLayer
{
    public class clsAdvisorNote
    {
        public int AdvisorNoteId { get; set; }
        public int StudentId { get; set; }
        public int AdvisorId { get; set; }
        public string NoteType { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public bool? HeadApproved { get; set; }
        public DateOnly NotedAt { get; set; }
    }
}
