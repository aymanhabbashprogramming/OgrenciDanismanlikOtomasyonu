namespace EntityLayer
{
    public class clsRequestCourse
    {
        public int RequestCourseId { get; set; }
        public int RequestId { get; set; }
        public string? TableGroup { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string? AttendanceStatus { get; set; }
        public string? Operation { get; set; }
        public string? PreviousGrade { get; set; }
        public int? CourseSemester { get; set; }
        public int? TheoryHours { get; set; }
        public int? PracticeHours { get; set; }
        public int? Ects { get; set; }
        public string? University { get; set; }
        public string? Department { get; set; }
    }
}
