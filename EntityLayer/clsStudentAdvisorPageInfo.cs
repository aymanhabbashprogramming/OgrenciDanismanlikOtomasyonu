namespace EntityLayer
{
    public class clsStudentAdvisorPageInfo
    {
        public int?      CurrentAdvisorId     { get; set; }
        public string?   AdvisorAcademicTitle { get; set; }
        public string?   AdvisorFirstName     { get; set; }
        public string?   AdvisorLastName      { get; set; }
        public string?   AdvisorEmail         { get; set; }
        public DateOnly? AssignmentStartDate  { get; set; }

        public List<clsOfficeHour>               OfficeHours       { get; set; } = new();
        public List<clsAdvisorAssignmentHistory>  AssignmentHistory { get; set; } = new();

        public bool HasCurrentAdvisor => CurrentAdvisorId.HasValue;

        public string? AdvisorFullName
        {
            get
            {
                if (AdvisorFirstName is null) return null;
                string namePart = $"{AdvisorFirstName} {AdvisorLastName}".Trim();
                return string.IsNullOrWhiteSpace(AdvisorAcademicTitle)
                    ? namePart
                    : $"{AdvisorAcademicTitle} {namePart}".Trim();
            }
        }
    }
}
