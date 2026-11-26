namespace EntityLayer
{
    public static class clsRequestStages
    {
        public const string Submitted              = "Gönderildi";
        public const string NotifiedToAdvisor      = "DanışmanaBildirildi";
        public const string AdvisorApproved        = "DanışmanOnayladı";
        public const string DepartmentHeadApproved = "BaşkanOnayladı";
        public const string Rejected               = "Reddedildi";
        public const string Completed              = "Tamamlandı";

        public static string GetDisplayLabel(string stage) => stage switch
        {
            Submitted              => "Gönderildi",
            NotifiedToAdvisor      => "Danışmana ulaştı",
            AdvisorApproved        => "Danışman onayı",
            DepartmentHeadApproved => "Bölüm başkanı onayı",
            Rejected               => "Reddedildi",
            Completed              => "Tamamlandı",
            _                      => stage
        };

        public static bool RequiresDepartmentHeadApproval(string formCode) =>
            formCode is "TF-ÖİŞL-02" or "TF-ÖİŞL-14";
    }
}
