namespace EntityLayer
{
    public class clsAdvisor
    {
        public int AdvisorId { get; set; }
        public int UserId { get; set; }
        public string SicilNo { get; set; } = string.Empty;
        public string? AcademicTitle { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? OfficeNo { get; set; }
    }
}
