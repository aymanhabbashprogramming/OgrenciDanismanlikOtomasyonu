namespace EntityLayer
{
    public class clsQuestionResultInfo
    {
        public int          SurveyQuestionId  { get; set; }
        public string       QuestionText      { get; set; } = string.Empty;
        public string       QuestionType      { get; set; } = string.Empty;
        public int          SortOrder         { get; set; }
        public int          AnswerCount       { get; set; }
        public double?      AverageScore      { get; set; }
        public int[]        ScoreDistribution { get; set; } = new int[10];
        public List<string> FreeTextAnswers   { get; set; } = new();
    }
}
