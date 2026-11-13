namespace EntityLayer
{
    public class clsSurveyParticipation
    {
        public int ParticipationId { get; set; }
        public int SurveyId { get; set; }
        public int StudentId { get; set; }
        public DateTime ParticipatedAt { get; set; }
    }
}
