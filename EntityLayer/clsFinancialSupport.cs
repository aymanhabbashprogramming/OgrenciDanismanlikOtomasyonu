namespace EntityLayer
{
    public class clsFinancialSupport
    {
        public int SupportId { get; set; }
        public int StudentId { get; set; }
        public string SupportType { get; set; } = string.Empty;
        public string? Provider { get; set; }
        public decimal? Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public int SemesterId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
