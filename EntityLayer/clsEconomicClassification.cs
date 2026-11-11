namespace EntityLayer
{
    public class clsEconomicClassification
    {
        public int ClassificationId { get; set; }
        public int StudentId { get; set; }
        public int SemesterId { get; set; }
        public string Classification { get; set; } = string.Empty;
        public int AdvisorId { get; set; }
        public string? Note { get; set; }
        public DateOnly ClassifiedAt { get; set; }
    }
}
