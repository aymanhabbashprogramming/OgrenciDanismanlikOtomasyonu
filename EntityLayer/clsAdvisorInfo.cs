namespace EntityLayer
{
    public class clsAdvisorInfo
    {
        public int    AdvisorId      { get; set; }
        public int    UserId         { get; set; }
        public string SicilNo        { get; set; } = string.Empty;
        public string AcademicTitle  { get; set; } = string.Empty;
        public string FirstName      { get; set; } = string.Empty;
        public string LastName       { get; set; } = string.Empty;
        public int    DepartmentId   { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string Username       { get; set; } = string.Empty;
        public bool   IsActive       { get; set; }

        public int    StudentCount  { get; set; }

        public string FullName => string.IsNullOrWhiteSpace(AcademicTitle)
            ? $"{FirstName} {LastName}".Trim()
            : $"{AcademicTitle} {FirstName} {LastName}".Trim();
    }
}
