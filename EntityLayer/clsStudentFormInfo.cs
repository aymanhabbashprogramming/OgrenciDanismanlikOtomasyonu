namespace EntityLayer
{
    public class clsStudentFormInfo
    {
        // ── Read-only student identity (from Students + Departments + Faculties) ──

        public int    StudentId       { get; set; }
        public string FullName        { get; set; } = string.Empty;
        public string StudentNumber   { get; set; } = string.Empty;
        public string NationalId      { get; set; } = string.Empty;
        public string DepartmentName  { get; set; } = string.Empty;
        public string FacultyName     { get; set; } = string.Empty;
        public string EducationType   { get; set; } = string.Empty;
        public string NationalityLabel { get; set; } = string.Empty;

        // ── Editable from Students table ──────────────────────────────────────────

        public string? MobilePhone { get; set; }
        public string? Email       { get; set; }

        // ── StudentFiles (0 = not yet filled) ────────────────────────────────────

        public int StudentFileId { get; set; }

        // Section 1
        public string?   BloodType      { get; set; }
        public string?   BirthPlace     { get; set; }
        public DateOnly? BirthDate      { get; set; }
        public string?   PreviousSchool { get; set; }
        public decimal?  Gano           { get; set; }

        // Section 2
        public string? HousingType            { get; set; }
        public string? HousingTypeDescription { get; set; }
        public string? Address                { get; set; }
        public string? FamilyAddress          { get; set; }
        public string? FamilyPhone            { get; set; }

        // Section 3
        public bool?   HasDisability          { get; set; }
        public int?    DisabilityRate         { get; set; }
        public string? DisabilityDescription  { get; set; }
        public bool?   HasChronicIllness      { get; set; }
        public string? ChronicIllnessDetails  { get; set; }
        public bool?   IsMartyrOrVeteranChild { get; set; }
        public bool?   StayedInCareHome       { get; set; }

        // Sections 4 & 5 — Parents
        public clsParent Mother { get; set; } = new() { Relation = "Anne" };
        public clsParent Father { get; set; } = new() { Relation = "Baba" };

        // Section 6
        public string?  ParentsStatus       { get; set; }
        public bool?    IsMotherDisabled    { get; set; }
        public bool?    IsFatherDisabled    { get; set; }
        public bool?    IsSiblingDisabled   { get; set; }
        public decimal? FamilyMonthlyIncome { get; set; }
        public string?  FamilyHomeType      { get; set; }
        public bool?    OwnsSummerHouse     { get; set; }
        public bool?    OwnsShop            { get; set; }
        public bool?    OwnsRentalHouse     { get; set; }
        public bool?    OwnsCar             { get; set; }
        public int?     SiblingsPrimary     { get; set; }
        public int?     SiblingsHighSchool  { get; set; }
        public int?     SiblingsUniInCity   { get; set; }
        public int?     SiblingsUniOutCity  { get; set; }

        // Section 7
        public bool?    ReceivesScholarship { get; set; }
        public decimal? ScholarshipAmount   { get; set; }
        public bool?    ReceivesLoan        { get; set; }
        public decimal? LoanAmount          { get; set; }
        public bool?    IsWorking           { get; set; }
        public decimal? WorkIncome          { get; set; }

        // ── Metadata ──────────────────────────────────────────────────────────────

        public DateTime? FilledAt  { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int TotalSiblings =>
            (SiblingsPrimary    ?? 0) +
            (SiblingsHighSchool ?? 0) +
            (SiblingsUniInCity  ?? 0) +
            (SiblingsUniOutCity ?? 0);
    }
}
