namespace EntityLayer
{
    public class clsAttachment
    {
        public int AttachmentId { get; set; }
        public int? MessageId { get; set; }
        public int? RequestId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string? ContentType { get; set; }
        public int? FileSize { get; set; }
        public int UploadedByUserId { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
