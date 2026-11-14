namespace EntityLayer
{
    public class clsSemesterWithPeriods
    {
        public clsSemester                  Semester       { get; set; } = new();
        public List<clsRequestPeriodInfo>   RequestPeriods { get; set; } = new();
    }
}
