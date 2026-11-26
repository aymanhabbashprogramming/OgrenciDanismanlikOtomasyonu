namespace EntityLayer
{
    public class clsRequestFormPreFillInfo
    {
        public int     StudentId          { get; set; }
        public int     AdvisorId          { get; set; }
        public int     CurrentSemesterId  { get; set; }
        public string  FullName           { get; set; } = string.Empty;
        public string  StudentNumber      { get; set; } = string.Empty;
        public string  NationalId         { get; set; } = string.Empty;
        public string  DepartmentName     { get; set; } = string.Empty;
        public string? MobilePhone        { get; set; }
        public string  EducationType      { get; set; } = string.Empty;
        public string? AdvisorFullName    { get; set; }

        public string? SemesterName    { get; set; }

        public bool HasAdvisor        => AdvisorId > 0;
        public bool HasCurrentSemester => CurrentSemesterId > 0;
    }
}
