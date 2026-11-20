namespace EntityLayer
{
    public class clsSurveyQuestion
    {
        public int    SurveyQuestionId { get; set; }
        public int    SurveyId         { get; set; }
        public string QuestionText     { get; set; } = string.Empty;
        public string QuestionType     { get; set; } = "Puanlama";
        public int    SortOrder        { get; set; }
    }
}
