namespace EntityLayer
{
    public class clsRequestTypeAvailability
    {
        public int      RequestTypeId { get; set; }
        public string   FormCode      { get; set; } = string.Empty;
        public string   FormName      { get; set; } = string.Empty;
        public bool     IsOpen        { get; set; }
        public DateOnly? NextStartDate { get; set; }
    }
}
