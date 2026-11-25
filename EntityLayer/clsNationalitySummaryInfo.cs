namespace EntityLayer
{
    public class clsNationalitySummaryInfo
    {
        public int                    DomesticCount     { get; set; }
        public int                    InternationalCount { get; set; }
        public List<clsLabelCountRow> TopCountries       { get; set; } = new();
    }
}
