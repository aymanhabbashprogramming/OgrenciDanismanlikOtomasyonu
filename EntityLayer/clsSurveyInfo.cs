namespace EntityLayer
{
    public class clsSurveyInfo
    {
        public int      SurveyId           { get; set; }
        public int      DepartmentId       { get; set; }
        public int      SemesterId         { get; set; }
        public string   SemesterName       { get; set; } = string.Empty;
        public string   Title              { get; set; } = string.Empty;
        public string?  Description        { get; set; }
        public string   Status             { get; set; } = string.Empty;
        public DateTime OpensAt            { get; set; }
        public DateTime ClosesAt           { get; set; }
        public int      QuestionCount      { get; set; }
        public int      ParticipationCount { get; set; }

        public bool IsOpen   => Status == "Yayınlandı" && DateTime.Now >= OpensAt && DateTime.Now <= ClosesAt;
        public bool HasEnded => Status == "Yayınlandı" && DateTime.Now > ClosesAt;
    }
}
