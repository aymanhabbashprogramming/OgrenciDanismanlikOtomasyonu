using DataAccessLayer;
using EntityLayer;

namespace BusinessLogicLayer
{
    public static class clsStudentEnrollmentStatusBusiness
    {
        public static readonly string[] AllStatuses =
        [
            "Aktif",
            "Kayıt Dondurma",
            "Erasmus",
            "Farabi",
            "Mevlana",
            "Mezun",
            "İlişik Kesme"
        ];

        public static bool IsExchangeStatus(string status) =>
            status == "Erasmus" || status == "Farabi" || status == "Mevlana";

        public static clsStudentEnrollmentStatus? GetCurrentStatus(int studentId) =>
            clsStudentEnrollmentStatusData.GetCurrentStatus(studentId);

        public static List<clsEnrollmentStatusHistory> GetStatusHistory(int studentId) =>
            clsStudentEnrollmentStatusData.GetStatusHistory(studentId);

        public static List<clsExchangeProgram> GetExchangeHistory(int studentId) =>
            clsStudentEnrollmentStatusData.GetExchangeHistory(studentId);

        public static bool ChangeStatus(int studentId, string newStatus, string? note,
                                        clsExchangeProgram? exchangeProgram,
                                        int managerDepartmentId)
        {
            clsStudent? student = clsStudentData.GetStudentInfoByID(studentId);
            if (student is null) return false;
            if (student.DepartmentId != managerDepartmentId) return false;

            if (!AllStatuses.Contains(newStatus)) return false;

            clsStudentEnrollmentStatus? current = clsStudentEnrollmentStatusData.GetCurrentStatus(studentId);
            if (current is not null && current.Status == newStatus) return false;

            if (IsExchangeStatus(newStatus))
            {
                if (exchangeProgram is null) return false;
                if (exchangeProgram.SemesterId <= 0) return false;
                if (!clsSemesterData.IsSemesterExist(exchangeProgram.SemesterId)) return false;
            }

            return clsStudentEnrollmentStatusData.ChangeStatus(studentId, newStatus, note, exchangeProgram);
        }
    }
}
