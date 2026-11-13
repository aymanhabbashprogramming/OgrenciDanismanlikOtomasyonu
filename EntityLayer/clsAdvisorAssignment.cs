namespace EntityLayer
{
    public class clsAdvisorAssignment
    {
        public int AssignmentId { get; set; }
        public int StudentId { get; set; }
        public int AdvisorId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? ChangeReason { get; set; }
    }
}
