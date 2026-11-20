namespace EntityLayer
{
    public class clsSurveyResultInfo
    {
        public clsSurveyInfo            Survey            { get; set; } = null!;
        public int                      AktifStudentCount { get; set; }
        public List<clsQuestionResultInfo> QuestionResults { get; set; } = new();
    }
}
