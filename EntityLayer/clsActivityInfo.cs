namespace EntityLayer
{
    public class clsActivityInfo
    {
        public int      ActivityId         { get; set; }
        public string   Title              { get; set; } = string.Empty;
        public string   ActivityType       { get; set; } = "Anket";
        public string   SemesterName       { get; set; } = string.Empty;
        public DateTime StartsAt           { get; set; }
        public DateTime EndsAt             { get; set; }
        public string   Status             { get; set; } = string.Empty;
        public int      TotalParticipants  { get; set; }
        public int      ParticipationCount { get; set; }
    }
}
