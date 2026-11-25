namespace EntityLayer
{
    public class clsDepartmentSummaryInfo
    {
        public List<clsLabelCountRow>    EnrollmentStatusCounts { get; set; } = new();
        public clsNationalitySummaryInfo NationalitySummary      { get; set; } = new();
        public List<clsLabelCountRow>    ClassYearCounts         { get; set; } = new();
        public int                       ActiveAdvisorCount       { get; set; }
        public int                       InactiveAdvisorCount     { get; set; }
    }
}
