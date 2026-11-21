using DataAccessLayer;
using EntityLayer;

namespace BusinessLogicLayer
{
    public static class clsDepartmentManagerBusiness
    {
        public static int GetManagerIdByUserId(int userId) =>
            clsDepartmentManagerData.GetManagerIdByUserId(userId);

        public static int GetDepartmentIdByUserId(int userId) =>
            clsDepartmentManagerData.GetDepartmentIdByUserId(userId);

        public static string GetFirstNameByUserId(int userId) =>
            clsDepartmentManagerData.GetFirstNameByUserId(userId);

        public static clsProfileCardInfo? GetManagerProfileInfoByUserId(int userId) =>
            clsDepartmentManagerData.GetManagerProfileInfoByUserId(userId);

        public static clsDepartmentSummaryInfo GetDepartmentSummaryByDepartmentId(int departmentId)
        {
            var (activeCount, inactiveCount) = clsDepartmentManagerData.GetAdvisorCountsByDepartmentId(departmentId);
            return new clsDepartmentSummaryInfo
            {
                EnrollmentStatusCounts = clsDepartmentManagerData.GetStudentCountsByEnrollmentStatusByDepartmentId(departmentId),
                NationalitySummary     = clsDepartmentManagerData.GetNationalitySummaryByDepartmentId(departmentId),
                ClassYearCounts        = clsDepartmentManagerData.GetStudentCountsByClassYearByDepartmentId(departmentId),
                ActiveAdvisorCount     = activeCount,
                InactiveAdvisorCount   = inactiveCount
            };
        }

        public static List<clsActivityInfo> GetActivitiesByDepartmentId(int departmentId)
        {
            List<clsSurveyInfo> surveys = clsSurveyData.GetAllSurveysByDepartment(departmentId);
            int totalActiveStudents     = clsStudentData.GetActiveStudentCountByDepartmentId(departmentId);

            return surveys.Select(survey => new clsActivityInfo
            {
                ActivityId         = survey.SurveyId,
                Title              = survey.Title,
                ActivityType       = "Anket",
                SemesterName       = survey.SemesterName,
                StartsAt           = survey.OpensAt,
                EndsAt             = survey.ClosesAt,
                Status             = _GetActivityStatus(survey),
                TotalParticipants  = totalActiveStudents,
                ParticipationCount = survey.ParticipationCount
            }).ToList();
        }

        private static string _GetActivityStatus(clsSurveyInfo survey)
        {
            if (survey.Status == "Taslak") return "Taslak";
            if (survey.HasEnded)           return "Sona Erdi";
            return "Yayında";
        }
    }
}
