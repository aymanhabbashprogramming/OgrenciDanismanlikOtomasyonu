using DataAccessLayer;
using EntityLayer;

namespace BusinessLogicLayer
{
    public static class clsDepartmentBusiness
    {
        public static clsDepartment? Find(int departmentId)
            => clsDepartmentData.GetDepartmentInfoByID(departmentId);

        public static List<clsDepartment> GetAllDepartments()
            => clsDepartmentData.GetAllDepartments();

        public static List<clsDepartment> GetDepartmentsByFacultyId(int facultyId)
            => clsDepartmentData.GetDepartmentsByFacultyId(facultyId);

        public static bool IsDepartmentExist(int departmentId)
            => clsDepartmentData.IsDepartmentExist(departmentId);
    }
}
