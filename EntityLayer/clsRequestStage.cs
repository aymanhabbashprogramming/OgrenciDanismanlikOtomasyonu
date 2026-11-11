namespace EntityLayer
{
    public class clsRequestStage
    {
        public int RequestStageId { get; set; }
        public int RequestId { get; set; }
        public string Stage { get; set; } = string.Empty;
        public DateTime StageAt { get; set; }
        public int ByUserId { get; set; }
        public string? Note { get; set; }
    }
}
