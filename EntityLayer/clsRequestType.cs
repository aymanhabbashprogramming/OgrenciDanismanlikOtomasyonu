namespace EntityLayer
{
    public class clsRequestType
    {
        public int RequestTypeId { get; set; }
        public string FormCode { get; set; } = string.Empty;
        public string FormName { get; set; } = string.Empty;
        public bool RequiresAdvisorApproval { get; set; }
    }
}
