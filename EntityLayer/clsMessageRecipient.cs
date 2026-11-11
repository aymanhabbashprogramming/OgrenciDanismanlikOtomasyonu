namespace EntityLayer
{
    public class clsMessageRecipient
    {
        public int MessageRecipientId { get; set; }
        public int MessageId { get; set; }
        public int RecipientUserId { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
    }
}
