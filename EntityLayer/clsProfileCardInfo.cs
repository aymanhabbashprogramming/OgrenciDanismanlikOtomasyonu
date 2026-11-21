namespace EntityLayer
{
    public class clsProfileCardInfo
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string FacultyName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string Username { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}".Trim();

        public string Initials
        {
            get
            {
                string first = FirstName.Length > 0 ? FirstName[0].ToString() : "";
                string last  = LastName.Length  > 0 ? LastName[0].ToString()  : "";
                return (first + last).ToUpperInvariant();
            }
        }
    }
}
