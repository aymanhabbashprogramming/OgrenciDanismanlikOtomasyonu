namespace EntityLayer
{
    public class clsMeetingAttendance
    {
        public int MeetingAttendanceId { get; set; }
        public int MeetingId { get; set; }
        public int StudentId { get; set; }
        public bool Attended { get; set; }
    }
}
