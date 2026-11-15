using DataAccessLayer;
using EntityLayer;

namespace BusinessLogicLayer
{
    public static class clsFacultyBusiness
    {
        public static clsFaculty? Find(int facultyId)
            => clsFacultyData.GetFacultyInfoByID(facultyId);

        public static List<clsFaculty> GetAllFaculties()
            => clsFacultyData.GetAllFaculties();

        public static bool IsFacultyExist(int facultyId)
            => clsFacultyData.IsFacultyExist(facultyId);
    }
}
