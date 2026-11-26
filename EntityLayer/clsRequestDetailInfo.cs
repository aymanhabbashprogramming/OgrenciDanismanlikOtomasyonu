namespace EntityLayer
{
    public class clsRequestDetailInfo
    {
        public int      RequestId       { get; set; }
        public int      RequestTypeId   { get; set; }
        public string   FormCode        { get; set; } = string.Empty;
        public string   FormName        { get; set; } = string.Empty;
        public string   SemesterName    { get; set; } = string.Empty;
        public DateTime SubmittedAt     { get; set; }
        public string?  Reason          { get; set; }
        public decimal? DeclaredGano    { get; set; }
        public string?  AdvisorFullName { get; set; }

        public List<clsRequestCourse> Courses { get; set; } = new();
        public List<clsRequestStage>  Stages  { get; set; } = new();

        public bool RequiresDepartmentHeadApproval =>
            clsRequestStages.RequiresDepartmentHeadApproval(FormCode);

        public string FinalStage
        {
            get
            {
                foreach (var stage in Stages)
                {
                    if (stage.Stage == clsRequestStages.Rejected  ||
                        stage.Stage == clsRequestStages.Completed)
                        return stage.Stage;
                }
                return string.Empty;
            }
        }
    }
}
