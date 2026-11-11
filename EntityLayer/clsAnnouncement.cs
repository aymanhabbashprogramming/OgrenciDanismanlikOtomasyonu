namespace EntityLayer
{
    public class clsAnnouncement
    {
        public int AnnouncementId { get; set; }
        public int AdvisorId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
