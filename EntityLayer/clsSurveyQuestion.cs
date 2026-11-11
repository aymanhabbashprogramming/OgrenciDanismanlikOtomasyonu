namespace EntityLayer
{
    public class clsSurveyQuestion
    {
        public int SurveyQuestionId { get; set; }
        public int SurveyId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }
}
