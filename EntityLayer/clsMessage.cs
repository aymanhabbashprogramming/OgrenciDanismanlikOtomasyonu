namespace EntityLayer
{
    public class clsMessage
    {
        public int MessageId { get; set; }
        public int SenderUserId { get; set; }
        public string? Subject { get; set; }
        public string Body { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public int? ParentMessageId { get; set; }
    }
}
