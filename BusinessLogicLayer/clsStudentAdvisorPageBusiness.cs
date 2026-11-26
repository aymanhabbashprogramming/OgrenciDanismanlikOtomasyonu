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
                pageInfo.OfficeHours =
                    clsStudentAdvisorPageData.GetOfficeHoursByAdvisorId(pageInfo.CurrentAdvisorId!.Value);
            }

            pageInfo.AssignmentHistory = clsStudentData.GetAdvisorHistory(studentId);

            return pageInfo;
        }
    }
}
