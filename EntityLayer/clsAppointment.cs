namespace EntityLayer
{
    public class clsAppointment
    {
        public int AppointmentId { get; set; }
        public int StudentId { get; set; }
        public int AdvisorId { get; set; }
        public DateTime AppointmentAt { get; set; }
        public string? Subject { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime BookedAt { get; set; }
    }
}
