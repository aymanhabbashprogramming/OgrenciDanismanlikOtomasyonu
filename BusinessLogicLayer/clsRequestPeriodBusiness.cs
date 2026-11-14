using DataAccessLayer;
using EntityLayer;

namespace BusinessLogicLayer
{
    public class clsRequestPeriodBusiness : clsRequestPeriod
    {
        public static List<clsRequestPeriodInfo> GetPeriodsBySemester(int semesterId) =>
            clsRequestPeriodData.GetPeriodsBySemester(semesterId);

        public static clsRequestPeriodInfo? GetOpenPeriod(int requestTypeId, DateOnly date) =>
            clsRequestPeriodData.GetOpenPeriod(requestTypeId, date);

        public static List<clsRequestPeriodInfo> GetUpcomingPeriods(int maxCount) =>
            clsRequestPeriodData.GetUpcomingPeriods(maxCount);

        public static bool IsRequestTypeOpen(string formCode) =>
            clsRequestPeriodData.IsRequestTypeOpen(formCode);
    }
}
