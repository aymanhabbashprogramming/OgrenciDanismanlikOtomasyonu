namespace EntityLayer
{
    public class clsExchangeProgram
    {
        public int ExchangeId { get; set; }
        public int StudentId { get; set; }
        public string ProgramName { get; set; } = string.Empty;
        public string? HostUniversity { get; set; }
        public string? Country { get; set; }
        public int SemesterId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
