namespace EntityLayer
{
    public class clsStudent
    {
        public int StudentId { get; set; }
        public int UserId { get; set; }
        public string StudentNumber { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string EducationType { get; set; } = string.Empty;
        public string? MobilePhone { get; set; }
        public string? Email { get; set; }
        public int? ClassYear { get; set; }
        public bool IsInternational { get; set; }
    }
}
