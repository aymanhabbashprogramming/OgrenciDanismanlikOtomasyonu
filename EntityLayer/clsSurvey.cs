namespace EntityLayer
{
    public class clsSurvey
    {
        public int SurveyId { get; set; }
        public int DepartmentId { get; set; }
        public int SemesterId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string SurveyType { get; set; } = string.Empty;
        public int? EvaluatedAdvisorId { get; set; }
        public DateTime OpensAt { get; set; }
        public DateTime ClosesAt { get; set; }
        public int CreatedByManagerId { get; set; }
    }
}
