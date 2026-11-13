namespace EntityLayer
{
    public class clsAdvisorAssignmentHistory
    {
        public int      AssignmentId   { get; set; }
        public int      AdvisorId      { get; set; }
        public string   AdvisorFullName { get; set; } = string.Empty;
        public DateOnly StartDate      { get; set; }
        public DateOnly? EndDate       { get; set; }
        public string?  ChangeReason   { get; set; }
    }
}
