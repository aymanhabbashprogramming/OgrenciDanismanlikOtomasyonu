namespace EntityLayer
{
    public class clsStudentEnrollmentStatus
    {
        public int EnrollmentStatusId { get; set; }
        public int StudentId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Note { get; set; }
    }
}
