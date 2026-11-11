namespace EntityLayer
{
    public class clsOfficeHour
    {
        public int OfficeHourId { get; set; }
        public int AdvisorId { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string? Location { get; set; }
        public int SemesterId { get; set; }
    }
}
