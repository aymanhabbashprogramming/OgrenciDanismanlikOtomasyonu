namespace EntityLayer
{
    public class clsStudentFile
    {
        public int StudentFileId { get; set; }
        public int StudentId { get; set; }
        public DateTime FilledAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? BloodType { get; set; }
        public string? BirthPlace { get; set; }
        public DateOnly? BirthDate { get; set; }
        public string? Nationality { get; set; }
        public string? PhotoPath { get; set; }
        public string? HousingType { get; set; }
        public string? Address { get; set; }
        public string? FamilyAddress { get; set; }
        public string? FamilyPhone { get; set; }
        public string? PreviousSchool { get; set; }
        public bool? HasDisability { get; set; }
        public int? DisabilityRate { get; set; }
        public bool? HasChronicIllness { get; set; }
        public string? ChronicIllnessDetails { get; set; }
        public bool? IsMartyrOrVeteranChild { get; set; }
        public bool? StayedInCareHome { get; set; }
        public decimal? Gano { get; set; }
        public string? ParentsStatus { get; set; }
        public bool? IsMotherDisabled { get; set; }
        public bool? IsFatherDisabled { get; set; }
        public bool? IsSiblingDisabled { get; set; }
        public decimal? FamilyMonthlyIncome { get; set; }
        public string? FamilyHomeType { get; set; }
        public bool? OwnsSummerHouse { get; set; }
        public bool? OwnsShop { get; set; }
        public bool? OwnsRentalHouse { get; set; }
        public bool? OwnsCar { get; set; }
        public int? SiblingsPrimary { get; set; }
        public int? SiblingsHighSchool { get; set; }
        public int? SiblingsUniInCity { get; set; }
        public int? SiblingsUniOutCity { get; set; }
        public bool? ReceivesScholarship { get; set; }
        public decimal? ScholarshipAmount { get; set; }
        public bool? ReceivesLoan { get; set; }
        public decimal? LoanAmount { get; set; }
        public bool? IsWorking { get; set; }
        public decimal? WorkIncome { get; set; }
    }
}
