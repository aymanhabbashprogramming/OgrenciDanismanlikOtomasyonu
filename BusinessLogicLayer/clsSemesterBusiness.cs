using DataAccessLayer;
using EntityLayer;

namespace BusinessLogicLayer
{
    public class clsSemesterBusiness : clsSemester
    {
        public string DisplayName => $"{AcademicYear} {Term}";

        private clsSemesterBusiness(clsSemester semester)
        {
            SemesterId   = semester.SemesterId;
            AcademicYear = semester.AcademicYear;
            Term         = semester.Term;
            StartDate    = semester.StartDate;
            EndDate      = semester.EndDate;
            IsCurrent    = semester.IsCurrent;
        }

        public static clsSemesterBusiness? Find(int semesterId)
        {
            clsSemester? semester = clsSemesterData.GetSemesterInfoByID(semesterId);
            return semester is null ? null : new clsSemesterBusiness(semester);
        }

        public static List<clsSemester> GetAllSemesters() =>
            clsSemesterData.GetAllSemesters();

        public static clsSemester? GetCurrentSemester() =>
            clsSemesterData.GetCurrentSemester();

        public static bool IsSemesterExist(int semesterId) =>
            clsSemesterData.IsSemesterExist(semesterId);
    }
}
