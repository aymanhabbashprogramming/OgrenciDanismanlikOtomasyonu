namespace EntityLayer
{
    public class clsStudentHomeInfo
    {
        public int      StudentId        { get; set; }
        public string   StudentNumber    { get; set; } = string.Empty;
        public string   FirstName        { get; set; } = string.Empty;
        public string   LastName         { get; set; } = string.Empty;
        public int      DepartmentId     { get; set; }
        public string   DepartmentName   { get; set; } = string.Empty;
        public string   FacultyName      { get; set; } = string.Empty;
        public int?     ClassYear        { get; set; }
        public string   EducationType    { get; set; } = string.Empty;
        public string?  EnrollmentStatus { get; set; }
        public string?  StudentEmail     { get; set; }
        public string?  MobilePhone      { get; set; }

        public int?     AdvisorId        { get; set; }
        public string?  AdvisorFullName  { get; set; }
        public string?  AdvisorEmail     { get; set; }

        public List<clsSurveyInfo> OpenUnansweredSurveys { get; set; } = new();

        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
