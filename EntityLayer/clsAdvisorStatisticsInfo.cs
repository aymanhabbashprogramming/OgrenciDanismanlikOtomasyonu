namespace EntityLayer
{
    public class clsAdvisorStatisticsInfo
    {
        public int    AdvisorId          { get; set; }
        public string FullName           { get; set; } = string.Empty;
        public bool   IsActive           { get; set; }
        public int    ActiveStudentCount { get; set; }
    }
}
