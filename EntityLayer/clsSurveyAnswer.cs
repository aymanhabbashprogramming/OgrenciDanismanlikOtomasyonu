namespace EntityLayer
{
    public class clsSurveyAnswer
    {
        public int SurveyAnswerId { get; set; }
        public int SurveyQuestionId { get; set; }
        public int Score { get; set; }
        public string? Comment { get; set; }
        public DateOnly AnsweredAt { get; set; }
    }
}
