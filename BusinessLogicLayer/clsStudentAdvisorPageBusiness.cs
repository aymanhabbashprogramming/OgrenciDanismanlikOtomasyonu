using DataAccessLayer;
using EntityLayer;

namespace BusinessLogicLayer
{
    public static class clsStudentAdvisorPageBusiness
    {
        public static clsStudentAdvisorPageInfo GetAdvisorPageInfo(int studentId)
        {
            clsStudentAdvisorPageInfo pageInfo =
                clsStudentAdvisorPageData.GetCurrentAdvisorWithStartDate(studentId);

            if (pageInfo.HasCurrentAdvisor)
            {
                int semesterId = clsSemesterBusiness.GetCurrentSemester()?.SemesterId ?? 0;
                pageInfo.OfficeHours = semesterId > 0
                    ? clsStudentAdvisorPageData.GetOfficeHoursByAdvisorId(
                          pageInfo.CurrentAdvisorId!.Value, semesterId)
                    : new List<clsOfficeHour>();
            }

            pageInfo.AssignmentHistory = clsStudentData.GetAdvisorHistory(studentId);

            return pageInfo;
        }
    }
}
