using DataAccessLayer;
using EntityLayer;

namespace BusinessLogicLayer
{
    public class clsDepartmentBusiness : clsDepartment
    {
        public enum enMode { AddNew, Update }
        public enMode Mode { get; private set; }

        public clsDepartmentBusiness()
        {
            DepartmentId = -1;
            Mode         = enMode.AddNew;
        }

        private clsDepartmentBusiness(clsDepartment d)
        {
            DepartmentId   = d.DepartmentId;
            DepartmentName = d.DepartmentName;
            FacultyId      = d.FacultyId;
            Mode           = enMode.Update;
        }

        public static clsDepartmentBusiness? Find(int departmentId)
        {
            clsDepartment? d = clsDepartmentData.GetDepartmentInfoByID(departmentId);
            return d is null ? null : new clsDepartmentBusiness(d);
        }

        private bool _AddNewDepartment()
        {
            DepartmentId = clsDepartmentData.AddNewDepartment(this);
            return DepartmentId != -1;
        }

        private bool _UpdateDepartment() => clsDepartmentData.UpdateDepartment(this);

        public bool Save()
        {
            DepartmentName = DepartmentName.Trim();
            if (string.IsNullOrEmpty(DepartmentName)) return false;
            if (FacultyId <= 0) return false;
            if (clsDepartmentData.IsDepartmentNameExistInFaculty(DepartmentName, FacultyId, DepartmentId)) return false;

            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewDepartment()) { Mode = enMode.Update; return true; }
                    return false;
                case enMode.Update:
                    return _UpdateDepartment();
                default:
                    return false;
            }
        }

        public static List<clsDepartment> GetAllDepartments() => clsDepartmentData.GetAllDepartments();

        public static bool DeleteDepartment(int departmentId)
            => clsDepartmentData.DeleteDepartment(departmentId);

        public static bool IsDepartmentExist(int departmentId)
            => clsDepartmentData.IsDepartmentExist(departmentId);

        public static bool IsDepartmentNameExistInFaculty(string name, int facultyId, int excludeId)
            => clsDepartmentData.IsDepartmentNameExistInFaculty(name, facultyId, excludeId);
    }
}
