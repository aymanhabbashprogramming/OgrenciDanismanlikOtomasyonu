namespace EntityLayer
{
    public class clsParent
    {
        public int ParentId { get; set; }
        public int StudentFileId { get; set; }
        public string Relation { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string? Occupation { get; set; }
        public bool? HasSocialSecurity { get; set; }
        public string? Workplace { get; set; }
        public decimal? MonthlyIncome { get; set; }
        public string? EducationLevel { get; set; }
        public bool? IsAlive { get; set; }
        public int? DeathYear { get; set; }
    }
}
