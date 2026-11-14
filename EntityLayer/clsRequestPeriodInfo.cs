namespace EntityLayer
{
    public class clsRequestPeriodInfo
    {
        public int      RequestPeriodId { get; set; }
        public int      SemesterId      { get; set; }
        public int      RequestTypeId   { get; set; }
        public string   FormCode        { get; set; } = string.Empty;
        public string   FormName        { get; set; } = string.Empty;
        public DateOnly StartDate       { get; set; }
        public DateOnly EndDate         { get; set; }
    }
}
