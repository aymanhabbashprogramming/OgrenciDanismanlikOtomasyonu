namespace EntityLayer
{
    public class clsRequestInfo
    {
        public int      RequestId     { get; set; }
        public int      RequestTypeId { get; set; }
        public string   FormCode      { get; set; } = string.Empty;
        public string   FormName      { get; set; } = string.Empty;
        public string   SemesterName  { get; set; } = string.Empty;
        public DateTime SubmittedAt   { get; set; }
        public string   CurrentStage  { get; set; } = string.Empty;

        public string CurrentStageDisplay => clsRequestStages.GetDisplayLabel(CurrentStage);
    }
}
