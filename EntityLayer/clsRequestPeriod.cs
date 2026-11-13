namespace EntityLayer
{
    public class clsRequestPeriod
    {
        public int RequestPeriodId { get; set; }
        public int SemesterId { get; set; }
        public int RequestTypeId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
